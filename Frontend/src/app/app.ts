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

  // Backend Basis URL anpassen
  apiBase = 'http://localhost:5286/api';

  from = 'ZRH';
  to = 'FRA';
  date = new Date().toISOString().slice(0, 10);

  loading = signal(false);
  error = signal<string | null>(null);
  rows = signal<Row[]>([]);
  raw = signal<any>(null);

  search(): void {
    this.error.set(null);
    this.rows.set([]);
    this.raw.set(null);

    if (this.from.length !== 3 || this.to.length !== 3 || !this.date) {
      this.error.set('Bitte gueltige Felder ausfuellen');
      return;
    }

    this.loading.set(true);

    const params = new HttpParams()
      .set('from', this.from.toUpperCase())
      .set('to', this.to.toUpperCase())
      .set('date', this.date);

    this.http.get(`${this.apiBase}/flights`, { params, responseType: 'text' }).subscribe({
      next: txt => {
        // Backend liefert String weiter
        const payload = tryParseJson(txt);
        this.raw.set(payload);

        const out: Row[] = [];
        const items = payload?.ScheduleResource?.Schedule ?? [];
        for (const it of items) {
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
}

function tryParseJson(t: string) {
  try { return JSON.parse(t); } catch { return t; }
}
