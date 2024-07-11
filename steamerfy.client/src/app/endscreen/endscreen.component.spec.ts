import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EndScreenComponent } from './endscreen.component';

describe('EndscreenComponent', () => {
  let component: EndScreenComponent;
  let fixture: ComponentFixture<EndScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EndScreenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EndScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
