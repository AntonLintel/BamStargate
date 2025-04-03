import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { Observable, of } from 'rxjs';
import { AppComponent, PeopleService, peopleResponse, duties } from './app.component';
import { QueryClient } from '@tanstack/angular-query-experimental';

describe('PeopleService', () => {
  let service: PeopleService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PeopleService]
    });
    service = TestBed.inject(PeopleService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch people successfully', () => {
    const mockResponse = { people: [{ id: 1, name: 'John Doe' }], success: true };

    service.getPeople().then(response => {
      expect(response.people.length).toBe(1);
      expect(response.people[0].name).toBe('John Doe');
    });

    const req = httpMock.expectOne('/api/GetPeople');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch sorted duties successfully', () => {
    const mockResponse = { astronautDuties: [{ id: 2 }, { id: 1 }], success: true };

    service.getDuties('John').subscribe(duties => {
      expect(duties.length).toBe(2);
      expect(duties[0].id).toBe(1);
      expect(duties[1].id).toBe(2);
    });

    const req = httpMock.expectOne('/api/GetDutiesByNameJohn');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  afterEach(() => {
    httpMock.verify();
  });
});
