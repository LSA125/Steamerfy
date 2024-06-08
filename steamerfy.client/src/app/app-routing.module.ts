import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LobbyComponent } from './lobby/lobby.component';
import { GameComponent } from './game/game.component';
import { EndScreenComponent } from './endscreen/endscreen.component';

const routes: Routes = [
  { path: '', redirectTo: '/lobby', pathMatch: 'full' },
  { path: 'lobby', component: LobbyComponent },
  { path: 'game/:id', component: GameComponent },
  { path: 'end', component: EndScreenComponent },
  { path: '**', redirectTo: '/lobby' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
