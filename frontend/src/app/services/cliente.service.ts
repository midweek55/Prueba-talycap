import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';

import { Cliente } from '../models/cliente.model';
import { environment } from '../../environments/environment';

const TIMEOUT_MS = 10000;

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private readonly baseUrl = `${environment.apiUrl}/clientes`;

  constructor(private http: HttpClient) {}

  buscarClientePorIdentificacion(identificacion: string): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.baseUrl}/${encodeURIComponent(identificacion)}`).pipe(
      timeout(TIMEOUT_MS),
      catchError((error: HttpErrorResponse | TimeoutError) => this.manejarError(error))
    );
  }

  private manejarError(error: HttpErrorResponse | TimeoutError): Observable<never> {
    let mensaje = 'Ocurrio un error inesperado. Intente nuevamente.';

    if (error instanceof TimeoutError) {
      mensaje = 'La solicitud tardo demasiado tiempo. Verifique su conexion e intente nuevamente.';
    } else if (error.status === 404) {
      mensaje = 'No se encontro ningun cliente con esa identificacion.';
    } else if (error.status === 0) {
      mensaje = 'No fue posible conectarse con el servidor. Verifique su conexion.';
    } else if (error.status >= 500) {
      mensaje = 'Ocurrio un error en el servidor. Intente nuevamente mas tarde.';
    } else if (error.error?.mensaje) {
      mensaje = error.error.mensaje;
    }

    return throwError(() => new Error(mensaje));
  }
}
