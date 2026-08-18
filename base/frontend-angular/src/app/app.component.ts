import { ChangeDetectionStrategy, Component } from '@angular/core';

import { BeneficiariosListaComponent } from './features/beneficiarios';
import { PlanosListaComponent } from './features/planos';

@Component({
  selector: 'app-root',
  imports: [PlanosListaComponent, BeneficiariosListaComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {}
