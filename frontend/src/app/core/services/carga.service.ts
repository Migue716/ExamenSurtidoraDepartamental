import { Injectable, computed, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CargaService {
  private readonly pendientes = signal(0);
  readonly visible = computed(() => this.pendientes() > 0);

  iniciar(): void {
    this.pendientes.update((valor) => valor + 1);
  }

  terminar(): void {
    this.pendientes.update((valor) => Math.max(0, valor - 1));
  }
}
