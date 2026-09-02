import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { Cliente } from '../../models/cliente.model';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-buscar-cliente',
  templateUrl: './buscar-cliente.component.html',
  styleUrls: ['./buscar-cliente.component.css']
})
export class BuscarClienteComponent implements OnDestroy {
  identificacion = '';
  cliente: Cliente | null = null;
  cargando = false;
  errorMensaje: string | null = null;
  intentoBusqueda = false;

  private busquedaSubscription: Subscription | null = null;

  constructor(private clienteService: ClienteService) {}

  buscar(): void {
    this.intentoBusqueda = true;
    this.errorMensaje = null;

    const valor = this.identificacion.trim();
    if (!valor) {
      this.cliente = null;
      return;
    }

    this.cargando = true;
    this.cliente = null;

    this.busquedaSubscription?.unsubscribe();
    this.busquedaSubscription = this.clienteService.buscarClientePorIdentificacion(valor).subscribe({
      next: (cliente) => {
        this.cliente = cliente;
        this.cargando = false;
      },
      error: (err: Error) => {
        this.errorMensaje = err.message;
        this.cargando = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.busquedaSubscription?.unsubscribe();
  }
}
