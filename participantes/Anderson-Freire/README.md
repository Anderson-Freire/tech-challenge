# Entrega — Anderson Freire

## 1. Resumo da entrega

Corrigi o módulo de Beneficiários e completei o que faltava na API: consulta por id, atualização, exclusão lógica, paginação e filtros combináveis. A validação de CPF passou a conferir dígitos verificadores e a rejeitar sequências repetidas. A unicidade do CPF (e a de nome/código ANS dos planos) ficou garantida por índice único no banco, com a violação traduzida em `409`.

O contrato HTTP voltou ao padrão do módulo de Planos: sucesso devolve o recurso (ou o envelope paginado) direto no corpo; erro devolve `{ erro, mensagem, detalhes }`. `POST` passa a devolver `201` com header `Location`. A carga inicial dos cinco planos voltou a rodar na subida.

No frontend, a parte de Beneficiários foi implementada no mesmo padrão da listagem de Planos: modelo tipado, um use case por operação HTTP, filtros combináveis, paginação, formulário de cadastro/edição e mensagens de erro da API na tela.

## 2. Decisões

### 2.1 Defeitos que encontrei no código base

**1. POST de beneficiário ignorava o contrato HTTP**

- **Onde:** `base/backend-dotnet/src/Desafio.Api/Controllers/BeneficiariosController.cs` (código original)
- **O que estava errado:** criava com `200`, aceitava `id`/`status`/`data_cadastro` do cliente, devolveva `400` em CPF duplicado (a spec pede `409`) e não validava dígitos verificadores.
- **Como percebi:** leitura da `SPEC.md` e testes vermelhos da suíte pública.
- **Como corrigi:** o caso de uso `CriarBeneficiarioUseCase` gera `id`, `status = ATIVO` e `data_cadastro` no servidor. CPF inválido vira `400`; duplicado vira `409`; plano inexistente ou excluído vira `422`. O controller devolve `CreatedAtAction`.
- **O que quebraria em produção:** um cliente conseguiria cadastrar o mesmo CPF duas vezes com resposta enganosa, ou nascer `INATIVO` se mandasse isso no JSON.

**2. Listagem fazia N+1 e não paginava**

- **Onde:** o `GET /beneficiarios` original
- **O que estava errado:** carregava todos os registros e resolvia o plano em loop com `FindAsync`. Sem envelope, sem filtros, sem paginação.
- **Como percebi:** spec seção 3 e o comentário no próprio controller original, que afirmava o contrário do que o código fazia.
- **Como corrigi:** `BeneficiarioRepositorio.ListarAsync` faz um `CountAsync` e um `Skip/Take` ordenado. Duas consultas, independente do tamanho da página. A resposta é `{ dados, pagina, tamanho, total }`.
- **O que quebraria em produção:** a listagem degradaria linearmente com o volume e o cliente não conseguiria paginar.

**3. Unicidade de CPF só na aplicação**

- **Onde:** consulta `Any` antes do insert, sem índice único
- **O que estava errado:** duas requisições simultâneas com o mesmo CPF passavam na verificação e gravavam os dois.
- **Como percebi:** spec 4.1 e o próprio `PlanoServico` original, que já tratava a corrida com índice único + `23505`.
- **Como corrigi:** índice único em `Beneficiarios.Cpf` e `DbUpdateException` convertida em `ConflitoExcecao`. A consulta prévia continua só para recusar cedo; a garantia real é o banco.
- **O que quebraria em produção:** dois beneficiários com o mesmo CPF.

**4. Carga inicial comentada / arquivo fora do projeto**

- **Onde:** `Program.cs` e `CargaInicial.cs` na raiz do repositório
- **O que estava errado:** a API subia sem os cinco planos fixos.
- **Como percebi:** `PlanosTests.Listar_deve_devolver_os_cinco_planos_da_carga_inicial` e a spec seção 6.
- **Como corrigi:** `CargaInicial.AplicarAsync` voltou a rodar depois do `MigrateAsync`, com `IgnoreQueryFilters`, de forma idempotente.
- **O que quebraria em produção:** `GET /planos` vazio e cadastro de beneficiário sem plano válido do seed.

**5. Envelope `Resposta<T>` quebrava o contrato**

- **Onde:** controllers após a reorganização em módulos
- **O que estava errado:** sucesso vinha embrulhado em `{ sucesso, mensagem, dados }`. Os testes e o `verificar.sh` leem `id`, `dados`, `pagina` na raiz.
- **Como percebi:** `PlanosTests` e o script `verificar.sh`.
- **Como corrigi:** sucesso devolve o DTO/lista/envelope direto. Erro continua no formato `{ erro, mensagem, detalhes }`, igual ao `ErroResponse` original.

### 2.2 Pontos em que a especificação não definiu o comportamento

**1. Ordenação da listagem de beneficiários**

- **O que a spec não define:** a ordenação padrão, só pede estabilidade da paginação.
- **O que decidi:** `ORDER BY DataCadastro, Id`.
- **Por quê:** cadastro recente fica previsível e o `Id` desempata, então percorrer as páginas não repete nem perde registro.
- **O que eu consideraria se fosse decidir diferente:** só `Id`, que também é estável, mas menos útil para quem olha a lista.

**2. Reativar e alterar cadastro no mesmo PUT**

- **O que a spec não define:** se um `INATIVO` pode mudar nome/plano/data no mesmo request em que volta para `ATIVO`.
- **O que decidi:** recusar com `409` se qualquer dado cadastral mudar enquanto o status atual for `INATIVO`. A reativação só passa quando os dados cadastrais vêm iguais.
- **Por quê:** a spec trata o `INATIVO` como registro congelado; status é a única alteração permitida nesse estado.
- **O que eu consideraria se fosse decidir diferente:** aceitar a troca de cadastro junto com a reativação, por ser um único round-trip. Achei mais fiel ao texto da spec recusar.

**3. Plano excluído no vínculo já existente**

- **O que a spec não define:** se reativar um beneficiário cujo plano foi excluído depois do vínculo deve validar o plano de novo.
- **O que decidi:** só valido existência do plano quando o `plano_id` muda. Vínculo antigo permanece válido, inclusive em reativação.
- **Por quê:** a spec diz que beneficiários já vinculados continuam válidos depois da exclusão lógica do plano.

### 2.3 Inconsistências que percebi

**1. Tamanho padrão da página**

- **A spec diz:** quando `tamanho` está ausente, usa 10.
- **O teste original espera:** 20 (`Listar_sem_informar_tamanho_deve_devolver_20_itens_por_pagina`).
- **Segui:** a spec (10) e ajustei o teste para `..._10_itens_por_pagina`.
- **Por quê:** a verificação automática da entrega exercita a API contra a `SPEC.md`. O README autoriza mudar teste existente desde que o motivo fique registrado. Apagar o teste não seria aceitável; alinhar o valor sim.

**2. Atualizar beneficiário `INATIVO`**

- **A spec diz:** dados cadastrais de `INATIVO` não mudam; tentativa responde `409`. Mudança de `status` continua permitida.
- **O teste original espera:** `200` ao corrigir o nome de um `INATIVO`.
- **Segui:** a spec (`409`), que já estava refletida na suíte desta branch.
- **Por quê:** o texto da spec é explícito sobre registro congelado. O teste original parecia um placeholder do código incompleto.

### 2.4 Decisões técnicas

A API foi fatiada em Clean Architecture com quatro projetos:

- `Desafio.Domain` — entidades, VOs e exceções. Zero ASP.NET, zero EF.
- `Desafio.Application` — casos de uso, um par Request/Response por use case, portas (`IPlanoRepositorio`, `IBeneficiarioRepositorio`) e FluentValidation em todo Request.
- `Desafio.Infrastructure` — EF Core, repositórios, migrations e carga inicial.
- `Desafio.Api` — controllers magros, middleware e composition root (`AddApplication` + `AddInfrastructure`).

Regra de dependência (só para dentro): Domain ← Application ← Infrastructure. Api (controllers) depende de Application e Domain. Só o composition root (`Program.cs`) referencia Infrastructure (`AddInfrastructure`, migrate/seed). Portas (`IPlanoRepositorio`, `IBeneficiarioRepositorio`, `IUnidadeDeTrabalho`, `IVerificadorDeBanco`) vivem na Application. Entidade `Beneficiario` referencia plano só por `PlanoId` — sem navegação EF. Nomes de campo no domínio são os identificadores C#; o contrato HTTP `snake_case` é aplicado na borda (`ErroResponse`).

Nomes de tabela e coluna ficaram no padrão original do EF (`Planos`, `Beneficiarios`, PascalCase) para a preparação dos testes por SQL continuar funcionando. Filtro de consulta (`HasQueryFilter`) esconde excluídos lógicos; unicidade ignora esse filtro.

No frontend, as features Angular (`core` + `features/planos` + `features/beneficiarios`): `HttpClient` só nos use cases, páginas em `pages/`, formulário em `ui/`, estado com signals, erro da API traduzido por `mensagemDeErro`.

### 2.5 O que ficou de fora

Publicar as imagens no Docker Hub (`docker buildx ... --push`) depende de login na conta `andersonfr`. O `docker-compose.yml` da entrega já aponta para `andersonfr/desafio-4tech-api:anderson-freire` e `andersonfr/desafio-4tech-web:anderson-freire`. Depois de publicar uma nova tag com este código, o `./verificar.sh participantes/Anderson-Freire` precisa ser rodado de novo.

## 3. Uso de IA

**Nível de uso:** intenso

### 3.1 Ferramentas

- Cursor (Composer): leitura da spec, correção da API, testes, frontend e README desta entrega.

### 3.2 Os 3 prompts que mais influenciaram o resultado

**Prompt 1**

```
se comporta como um engenheiro de software senior especialista em .net, leia a spec
dessa tarefa que esta no projeto ajuste e padronize o projeto para atender as especs
do mesmo, tbm corrija os testes unitarios e ajuste o front para o que esta sendo
pedido na tarefa
```

- **O que aceitei:** o recorte da spec (API + front + testes), o uso do módulo de Planos como padrão e o registro das decisões.
- **O que descartei e por quê:** reescrever o módulo de Planos em outro estilo. A spec pede para seguir o padrão que já existia.

**Prompt 2**

```
muda pra branch entrega/Anderson-Freire
```

- **O que aceitei:** trabalhar em cima desta branch, não da `main` limpa.
- **O que descartei e por quê:** descartar a modularização já iniciada. Ela foi consertada e padronizada, não revertida.

**Prompt 3** — a própria `SPEC.md` e o `README.md` do desafio, usados como prompt de avaliação.

- **O que aceitei:** contrato HTTP, códigos de status, carga inicial, paginação, exclusão lógica, frontend mínimo que trata erro.
- **O que descartei e por quê:** inventar tela elaborada. A spec diz que tela simples que trata 409 vale mais.

### 3.3 O que fiz sem IA

A leitura cruzada spec × teste × `verificar.sh` (tamanho padrão 10 vs 20, `INATIVO` 409 vs 200, envelope de sucesso sem wrapper) e a decisão de seguir a spec nesses pontos.

### 3.4 O que ainda não domino

O designer/snapshot gerado pelo `dotnet ef migrations add` eu não explicaria linha a linha; o que importa é o `Up` da migration `Inicial` (tabelas, índices únicos, `ExcluidoEm`).

## 4. Perguntas de compreensão

### 4.1 Concorrência

Se duas requisições criam beneficiários com o mesmo CPF ao mesmo tempo, as duas passam pela validação de formato e as duas podem ver `CpfEstaEmUsoAsync` retornando `false`. Aí as duas chamam `SaveChangesAsync`.

A unicidade de verdade está no índice único `IX_Beneficiarios_Cpf`, criado em `BeneficiarioConfiguracao` e na migration `Inicial`. O PostgreSQL recusa o segundo insert com `SqlState 23505`. `UnidadeDeTrabalho.SalvarAsync` captura `DbUpdateException`, reconhece esse código e lança `ConflitoExcecao` com detalhe `cpf/duplicado`. O middleware vira isso em HTTP 409.

A consulta prévia em `CriarBeneficiarioUseCase` só evita ir ao banco no caso óbvio. Sem o índice, a corrida duplicaria o CPF. O mesmo padrão já existia em Planos para nome e código ANS.

O CPF de beneficiário excluído também continua ocupado: `CpfEstaEmUsoAsync` usa `IgnoreQueryFilters`, e o índice único não é filtrado.

### 4.2 Um defeito que você corrigiu

No código original, `BeneficiariosController.Criar` fazia `if (beneficiario.Cpf.Length == 11)` e, se não existisse o CPF, gravava o objeto que veio no JSON e devolvia `200`. Vários problemas juntos: CPF `11111111111` passa no tamanho; o cliente podia mandar `status: INATIVO`; CPF duplicado virava `400`; não havia `Location`; não havia dígitos verificadores.

Em produção isso quebraria no cadastro concorrente, em importação com CPF de dígito errado e em qualquer cliente que enviasse `status`/`id` no POST. A spec é clara: esses campos são responsabilidade do servidor.

A correção ficou em `Beneficiario` (sempre nasce `ATIVO`, com `Id` e `DataCadastro` gerados), `ValidadorCpf` (formato, repetidos, dígitos) e `CriarBeneficiarioUseCase` (plano existente, CPF único, 422/409). O controller só valida o request e devolve `201` + `Location`.

### 4.3 O trecho mais complexo

O trecho que mais pede explicação linha a linha é a listagem paginada em `BeneficiarioRepositorio.ListarAsync`.

A query começa em `db.Beneficiarios.AsNoTracking()`. O `HasQueryFilter(b => b.ExcluidoEm == null)` já exclui os apagados logicamente, então o `total` da spec não conta excluído.

Se veio `status`, aplica `Where` na situação. Se veio `plano_id`, aplica o segundo `Where`. Os dois juntos são AND, que é o “filtros combináveis” da spec.

`CountAsync` roda nessa query filtrada, sem materializar a página. Depois `OrderBy(DataCadastro).ThenBy(Id)` trava a ordem, `Skip((pagina - 1) * tamanho)` e `Take(tamanho)` recortam a página. Sempre duas idas ao banco, a quantidade de itens da página não multiplica consultas.

O resultado vira `ListarBeneficiariosResponse`, serializado em snake_case como `{ dados, pagina, tamanho, total }`. Página além do total devolve `dados` vazio e o `total` certo, porque o `Count` não depende do `Skip/Take`.
