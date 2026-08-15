import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { CargaService } from '../../../core/services/carga.service';
import { Cliente } from '../../models/cliente.model';
import { ClienteService } from '../../services/cliente.service';
import {
  ConfirmacionDialogoComponent,
  ConfirmacionDialogoData
} from '../../../shared/confirmacion-dialogo/confirmacion-dialogo.component';

@Component({
  selector: 'app-cliente-lista',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatSelectModule,
    MatTableModule
  ],
  templateUrl: './cliente-lista.component.html',
  styleUrl: './cliente-lista.component.scss'
})
export class ClienteListaComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly servicio = inject(ClienteService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);
  readonly carga = inject(CargaService);

  readonly columnas = ['clienteId', 'cliente', 'correoElectronico', 'telefono', 'acciones'];
  readonly clientes = signal<Cliente[]>([]);
  readonly total = signal(0);
  readonly pagina = signal(0);
  readonly tamanioPagina = signal(10);

  readonly filtros = this.fb.nonNullable.group({
    buscar: [''],
    activo: ['true']
  });

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    const { buscar, activo } = this.filtros.getRawValue();
    this.servicio
      .consultar({
        pagina: this.pagina() + 1,
        tamanioPagina: this.tamanioPagina(),
        buscar,
        activo: activo === '' ? null : activo === 'true',
        ordenarPor: 'apellidoPaterno'
      })
      .subscribe((resultado) => {
        this.clientes.set(resultado.items);
        this.total.set(resultado.totalRegistros);
      });
  }

  buscar(): void {
    this.pagina.set(0);
    this.cargar();
  }

  limpiar(): void {
    this.filtros.reset({ buscar: '', activo: 'true' });
    this.pagina.set(0);
    this.cargar();
  }

  cambiarPagina(evento: PageEvent): void {
    this.pagina.set(evento.pageIndex);
    this.tamanioPagina.set(evento.pageSize);
    this.cargar();
  }

  nombreCompleto(cliente: Cliente): string {
    return [cliente.nombre, cliente.apellidoPaterno, cliente.apellidoMaterno]
      .filter((parte) => !!parte)
      .join(' ');
  }

  editar(cliente: Cliente): void {
    this.router.navigate(['/clientes', cliente.clienteId, 'editar']);
  }

  darDeBaja(cliente: Cliente): void {
    this.dialog
      .open(ConfirmacionDialogoComponent, {
        data: {
          titulo: 'Dar de baja',
          mensaje: `¿Confirma la baja lógica de ${this.nombreCompleto(cliente)}? El registro no se eliminará físicamente.`,
          textoConfirmar: 'Dar de baja'
        } satisfies ConfirmacionDialogoData
      })
      .afterClosed()
      .subscribe((confirmado) => {
        if (!confirmado) {
          return;
        }

        this.servicio.darDeBaja(cliente.clienteId).subscribe(() => {
          this.snackBar.open('El cliente fue dado de baja.', 'Cerrar', { duration: 3000 });
          this.cargar();
        });
      });
  }
}
