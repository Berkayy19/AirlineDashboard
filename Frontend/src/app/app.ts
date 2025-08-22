import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';

type Row = { flight: string; dep: string; arr: string; op: string };

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  private http = inject(HttpClient);

  // Bei Proxy: '/api', sonst 'http://localhost:5286/api'
  apiBase = 'http://localhost:5286/api';

  from = 'ZRH';
  to = 'FRA';
  date = new Date().toISOString().slice(0, 10);

  loading = signal(false);
  error = signal<string | null>(null);
  rows = signal<Row[]>([]);
  raw = signal<any>(null);

  // Details
  selectedFlight: any = null; // einzelnes Flight-Objekt
  loadingDetails = signal(false);

  search(): void {
    this.error.set(null);
    this.rows.set([]);
    this.raw.set(null);
    this.selectedFlight = null;

    if (this.from.length !== 3 || this.to.length !== 3 || !this.date) {
      this.error.set('Bitte gueltige Felder ausfuellen');
      return;
    }

    this.loading.set(true);

    const params = new HttpParams()
      .set('from', this.from.toUpperCase())
      .set('to', this.to.toUpperCase())
      .set('date', this.date);

    // Backend antwortet JSON -> direkt als Objekt holen
    this.http.get<any>(`${this.apiBase}/flights`, { params }).subscribe({
      next: payload => {
        this.raw.set(payload);

        const schedules = ensureArray(payload?.ScheduleResource?.Schedule);
        const out: Row[] = [];
        for (const it of schedules) {
          const f = it?.Flight ?? {};
          const flight = `${f?.MarketingCarrier?.AirlineID ?? ''}${f?.MarketingCarrier?.FlightNumber ?? ''}`;
          const dep = `${f?.Departure?.AirportCode ?? ''} ${f?.Departure?.ScheduledTimeLocal?.DateTime ?? ''}`;
          const arr = `${f?.Arrival?.AirportCode ?? ''} ${f?.Arrival?.ScheduledTimeLocal?.DateTime ?? ''}`;
          const op = f?.OperatingCarrier?.AirlineID ?? f?.MarketingCarrier?.AirlineID ?? '';
          out.push({ flight, dep, arr, op });
        }
        this.rows.set(out);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(err?.error?.message ?? 'Fehler beim Laden');
        this.loading.set(false);
      }
    });
  }

  loadDetails(flightCode: string) {
    this.loadingDetails.set(true);
    const params = new HttpParams()
      .set('flight', flightCode)
      .set('date', this.date);

    this.http.get<any>(`${this.apiBase}/flights/by-number`, { params })
      .subscribe({
        next: payload => {
          // FlightStatusResource -> Flights -> Flight kann Array oder Objekt sein
          const flights = payload?.FlightStatusResource?.Flights?.Flight;
          this.selectedFlight = Array.isArray(flights) ? flights[0] : flights ?? null;
          this.loadingDetails.set(false);
        },
        error: err => {
          this.error.set(err?.error?.message ?? 'Fehler beim Laden der Fluginfos');
          this.loadingDetails.set(false);
        }
      });
  }
}

function ensureArray<T>(x: T | T[] | undefined | null): T[] {
  if (!x) return [];
  return Array.isArray(x) ? x : [x];
}
