import { TestBed } from '@angular/core/testing';

import { ScenariosService } from './scenarios.service';

describe('ScenariosService', () => {
  let service: ScenariosService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ScenariosService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
