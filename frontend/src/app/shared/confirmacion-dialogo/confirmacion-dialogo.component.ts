import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

export interface ConfirmacionDialogoData {
  titulo: string;
  mensaje: string;
  textoConfirmar?: string;
}

@Component({
  selector: 'app-confirmacion-dialogo',
  standalone: true,
  imports: [MatDialogModule],
  template: `
    <h2 mat-dialog-title>{{ data.titulo }}</h2>
    <mat-dialog-content>
      <p>{{ data.mensaje }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button class="btn btn-secundario" type="button" (click)="dialogo.close(false)">Cancelar</button>
      <button class="btn btn-primario" type="button" (click)="dialogo.close(true)">
        {{ data.textoConfirmar || 'Confirmar' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: `
    h2 { font-size: 1.1rem; font-weight: 600; }
    p { margin: 0.4rem 0 0.75rem; color: #64748b; line-height: 1.5; }
    mat-dialog-actions { gap: 0.5rem; padding: 0 1rem 1rem; }
  `
})
export class ConfirmacionDialogoComponent {
  readonly dialogo = inject(MatDialogRef<ConfirmacionDialogoComponent, boolean>);
  readonly data = inject<ConfirmacionDialogoData>(MAT_DIALOG_DATA);
}
