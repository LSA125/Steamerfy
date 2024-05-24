import { Component, Input } from '@angular/core';
import { Player } from '../../models/GameHub/player';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() players: Player[] = [];
  showSidebar: boolean = true;

  onToggleSidebar() {
    this.showSidebar = !this.showSidebar;
  }
}
