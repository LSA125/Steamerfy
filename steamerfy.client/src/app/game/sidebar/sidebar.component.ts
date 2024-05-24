import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Player } from '../../models/GameHub/player';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() players: Player[] = [];
  public showSidebar: boolean = true;
  @Output() onSidebarChange = new EventEmitter < boolean>();

  onToggleSidebar() {
    this.showSidebar = !this.showSidebar;
    this.onSidebarChange.emit(this.showSidebar);
  }
}
