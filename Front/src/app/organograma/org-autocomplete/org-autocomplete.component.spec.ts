import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgAutocompleteComponent } from './org-autocomplete.component';

describe('OrgAutocompleteComponent', () => {
  let component: OrgAutocompleteComponent;
  let fixture: ComponentFixture<OrgAutocompleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrgAutocompleteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrgAutocompleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
