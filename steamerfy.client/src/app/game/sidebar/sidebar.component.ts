import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Player } from '../../models/GameHub/player';
import { GameService } from '../../game.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  public showSidebar: boolean = true;
  @Output() onSidebarChange = new EventEmitter<boolean>();

  public canStart: boolean = true;
  public canSkip: boolean = false;
  constructor(public gs: GameService) {
    this.gs.questionStarted$.subscribe(() => { this.canSkip = true; });
    this.gs.questionEnded$.subscribe(() => { this.canSkip = false; });
  }

  onToggleSidebar() {
    this.showSidebar = !this.showSidebar;
    this.onSidebarChange.emit(this.showSidebar);
  }

  StartNextQuestion() {
    if (this.canSkip) {
      this.gs.EndQuestion();
    } 
    else if (this.canStart) {
      this.canStart = false;
      setTimeout(() => {
        this.canStart = true;
      }, 3000);
      this.gs.StartQuestion();
    }
  }
}
