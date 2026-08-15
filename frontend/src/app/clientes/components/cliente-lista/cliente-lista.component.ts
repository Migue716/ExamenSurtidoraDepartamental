import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
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
    MatIconModule,
    MatPaginatorModule,
    MatSortModule,
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
  private readonly destroyRef = inject(DestroyRef);

  readonly columnas = ['clienteId', 'cliente', 'correoElectronico', 'telefono', 'estado', 'acciones'];
  readonly clientes = signal<Cliente[]>([]);
  readonly total = signal(0);
  readonly pagina = signal(0);
  readonly tamanioPagina = signal(10);
  readonly ordenarPor = signal('apellidoPaterno');
  readonly descendente = signal(false);

  readonly filtros = this.fb.nonNullable.group({
    buscar: [''],
    activo: ['true']
  });

  get etiquetaEstado(): string {
    const valor = this.filtros.controls.activo.value;
    if (valor === 'false') {
      return 'Inactivos';
    }
    if (valor === '') {
      return 'Todos';
    }
    return 'Activos';
  }

  columnasVisibles(): string[] {
    return this.clientes().length === 0
      ? ['clienteId', 'cliente', 'correoElectronico', 'telefono', 'estado']
      : this.columnas;
  }

  ngOnInit(): void {
    this.filtros.controls.buscar.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.buscar());

    this.filtros.controls.activo.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.buscar());

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
        ordenarPor: this.ordenarPor(),
        descendente: this.descendente()
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
    this.filtros.reset({ buscar: '', activo: 'true' }, { emitEvent: false });
    this.pagina.set(0);
    this.cargar();
  }

  cambiarPagina(evento: PageEvent): void {
    this.pagina.set(evento.pageIndex);
    this.tamanioPagina.set(evento.pageSize);
    this.cargar();
  }

  cambiarOrden(evento: Sort): void {
    const mapa: Record<string, string> = {
      clienteId: 'id',
      cliente: 'apellidoPaterno',
      correoElectronico: 'correo'
    };

    this.ordenarPor.set(evento.direction ? mapa[evento.active] ?? 'apellidoPaterno' : 'apellidoPaterno');
    this.descendente.set(evento.direction === 'desc');
    this.pagina.set(0);
    this.cargar();
  }

  nombreCompleto(cliente: Cliente): string {
    return [cliente.nombre, cliente.apellidoPaterno, cliente.apellidoMaterno]
      .filter((parte) => !!parte)
      .join(' ');
  }

  editar(cliente: Cliente): void {
    void this.router.navigate(['/clientes', cliente.clienteId, 'editar']);
  }

  darDeBaja(cliente: Cliente): void {
    this.dialog
      .open(ConfirmacionDialogoComponent, {
        width: '420px',
        data: {
          titulo: 'Dar de baja',
          mensaje: `¿Confirma la baja lógica de ${this.nombreCompleto(cliente)}? El registro permanecerá en el sistema como inactivo.`,
          textoConfirmar: 'Dar de baja'
        } satisfies ConfirmacionDialogoData
      })
      .afterClosed()
      .subscribe((confirmado) => {
        if (!confirmado) {
          return;
        }

        this.servicio.darDeBaja(cliente.clienteId).subscribe(() => {
          this.snackBar.open('El cliente fue dado de baja.', 'Cerrar', { panelClass: 'snack-ok' });
          this.cargar();
        });
      });
  }
}
