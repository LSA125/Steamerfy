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


  constructor(public gs: GameService) {
  }

  onToggleSidebar() {
    this.showSidebar = !this.showSidebar;
    this.onSidebarChange.emit(this.showSidebar);
  }
}
