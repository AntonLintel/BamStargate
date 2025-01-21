import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { AppComponent, PeopleService } from './app.component';
import { QueryClient } from '@tanstack/angular-query-experimental';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let peopleService: PeopleService;
  let queryClient: QueryClient;

  beforeEach(async () => {
    // Mock CurrentQuery
    const currentQueryMock = {
      setOptions: jasmine.createSpy('setOptions')
    };

    // Mock QueryObserver
    const queryObserverMock = {
      setOptions: jasmine.createSpy('setOptions'),
      currentQuery: currentQueryMock
    };

    // Mock QueryCache
    const queryCacheMock = {
      build: jasmine.createSpy('build').and.returnValue(queryObserverMock)
    };

    // Mock QueryClient
    const queryClientMock = {
      getQueryData: jasmine.createSpy('getQueryData').and.returnValue({
        people: [{ id: 1, name: 'John Doe', currentRank: 'Captain', currentDutyTitle: 'Commander', careerStartDate: '2020-01-01', careerEndDate: '2025-01-01' }]
      }),
      fetchQuery: jasmine.createSpy('fetchQuery').and.returnValue(Promise.resolve({
        people: [{ id: 1, name: 'John Doe', currentRank: 'Captain', currentDutyTitle: 'Commander', careerStartDate: '2020-01-01', careerEndDate: '2025-01-01' }]
      })),
      defaultQueryOptions: jasmine.createSpy('defaultQueryOptions').and.returnValue({}),
      getQueryCache: jasmine.createSpy('getQueryCache').and.returnValue(queryCacheMock)
    };

    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        MatToolbarModule,
        MatIconModule,
        MatButtonModule,
        MatFormFieldModule,
        MatSelectModule
      ],
      providers: [
        PeopleService,
        { provide: QueryClient, useValue: queryClientMock }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    peopleService = TestBed.inject(PeopleService);
    queryClient = TestBed.inject(QueryClient);

    spyOn(peopleService, 'getPeople').and.returnValue(Promise.resolve({
      people: [{ id: 1, name: 'John Doe', currentRank: 'Captain', currentDutyTitle: 'Commander', careerStartDate: '2020-01-01', careerEndDate: '2025-01-01' }],
      success: true
    }));
    spyOn(peopleService, 'getDuties').and.returnValue(of([]));
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
});
