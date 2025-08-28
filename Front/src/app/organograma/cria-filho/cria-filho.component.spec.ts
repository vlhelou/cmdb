import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CriaFilhoComponent } from './cria-filho.component';

describe('CriaFilhoComponent', () => {
  let component: CriaFilhoComponent;
  let fixture: ComponentFixture<CriaFilhoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CriaFilhoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CriaFilhoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
