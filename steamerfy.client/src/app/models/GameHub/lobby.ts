import { Player } from './player';
import { Question } from './question';

export interface Lobby {
  id: number;
  players: Player[];
  createdAt: Date;
  currentQuestion: Question;
}
