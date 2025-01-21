import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Injectable, inject } from '@angular/core';
import { lastValueFrom, Observable, of, BehaviorSubject } from 'rxjs'
import { switchMap, map, finalize, tap, catchError } from 'rxjs/operators';
import { injectQuery, QueryClient } from '@tanstack/angular-query-experimental'
import { FormControl} from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})


export class AppComponent {
  peopleService = inject(PeopleService)
  queryClient = inject(QueryClient)

  selectedPersonId = new FormControl<number | null>(null);
  selectedPersonName: string | null = null;
  duties$: Observable<duties[]>

  loading$ = new BehaviorSubject<boolean>(false);

  constructor() {
    this.duties$ = this.selectedPersonId.valueChanges.pipe(
      tap(() => this.loading$.next(true)),
      switchMap(id => {
        if (id !== null) {
          const person = this.query.data()?.people.find(person => person.id === id);
          if (person) {
            this.selectedPersonName = person.name;
            return this.peopleService.getDuties(this.selectedPersonName).pipe(
              catchError(err => { console.error(`Error fetching duties: ${err}`);
              return of([]); }),
            finalize(() => this.loading$.next(false)));
          }
        }
        this.selectedPersonName = null;
        this.loading$.next(false);
        return [];
      }),
      map(duties => duties || []) // handle empty lists
    );
  }

  query = injectQuery(() => ({
    queryKey: ['people'],
    queryFn: () => this.peopleService.getPeople(),
  }))
}

@Injectable({ providedIn: 'root' })
export class PeopleService {
  private http = inject(HttpClient)

  getPeople(): Promise<peopleResponse> {
    return lastValueFrom(
      this.http.get<peopleResponse>('/api/GetPeople'),
    )
  }

  getDuties(name: string): Observable<duties[]> {
    return this.http.get<dutyResponse>(`/api/GetDutiesByName${name}`).pipe(
      map(response => {
        return response.astronautDuties.sort(a => a.id)
      }));
  }
}

interface peopleResponse {
  people: Array<people>
  success: boolean
}

interface dutyResponse {
  astronautDuties: Array<duties>
  success: boolean
}

interface people {
  id: number
  name: string
  currentRank: string
  currentDutyTitle: string
  careerStartDate: string
  careerEndDate: string
}

interface duties {
  id: number
  personId: number
  rank: string
  dutyTitle: string
  dutyStartDate: string
  dutyEndDate: string
}
