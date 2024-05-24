import { AnswerData } from './answerdata';
import { Player } from './player';
import { Question } from './question';

export interface LobbyCreationResponse {
  lobbyId: number;
}

export interface LobbyJoinResponse {
  player: Player;
}

export interface QuestionResponse {
  question: Question;
}

export interface GameEndedResponse {
  answerData: AnswerData[];
}

export interface LeaveLobbyResponse {
  player: Player;
}
