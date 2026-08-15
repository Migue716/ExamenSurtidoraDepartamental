import { Routes } from '@angular/router';
import { ClienteListaComponent } from './clientes/components/cliente-lista/cliente-lista.component';
import { ClienteFormularioComponent } from './clientes/components/cliente-formulario/cliente-formulario.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'clientes' },
  { path: 'clientes', component: ClienteListaComponent },
  { path: 'clientes/nuevo', component: ClienteFormularioComponent },
  { path: 'clientes/:id/editar', component: ClienteFormularioComponent },
  { path: '**', redirectTo: 'clientes' }
];
