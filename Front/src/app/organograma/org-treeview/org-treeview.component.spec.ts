import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgTreeviewComponent } from './org-treeview.component';

describe('OrgTreeviewComponent', () => {
  let component: OrgTreeviewComponent;
  let fixture: ComponentFixture<OrgTreeviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrgTreeviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrgTreeviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
