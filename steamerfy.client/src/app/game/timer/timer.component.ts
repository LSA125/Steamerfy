import { Component, Input } from '@angular/core';
import { GameService } from '../../game.service';

@Component({
  selector: 'app-timer',
  templateUrl: './timer.component.html',
  styleUrls: ['./timer.component.css']
})
export class TimerComponent {
  public time: number = 0;
  public MaxTime: number = 0;
  @Input() timerSize: number = 200;
  private interval: any;

  constructor(private gs: GameService) {
    gs.questionStarted$.subscribe((question) => this.onNewQuestion(new Date(question.ExpireTime)));
    gs.newGameState$.subscribe((state) => {
      if (state.CurrentQuestion)
        this.onNewQuestion(new Date(state.CurrentQuestion.ExpireTime));
    });
  }

  public onNewQuestion(expireTime: Date) {
    const now = new Date();
    this.MaxTime = Math.round((expireTime.getTime() - now.getTime()) / 1000);
    this.startTimer();
  }

  public startTimer() {
    if (this.interval) {
      clearInterval(this.interval);
    }
    this.time = this.MaxTime;
    this.setSVGAnimationDuration();
    this.resetSVGAnimation();

    this.interval = setInterval(() => {
      this.time--;
      if (this.time == 0) {
        if (this.gs.isUserHost()) {
          console.log("Ending question")
          this.gs.EndQuestion();
        }
        clearInterval(this.interval);
      }
    }, 1000);
  }

  private setSVGAnimationDuration() {
    const circle = document.querySelector('.circle') as SVGCircleElement;
    if (circle) {
      console.log("setting animation duration");
      const radius = circle.r.baseVal.value;
      const circumference = 2 * Math.PI * radius;
      circle.style.strokeDasharray = `${circumference}`;
      circle.style.strokeDashoffset = `${circumference}`;
      circle.style.animation = `circletimer ${this.MaxTime}s linear`;
    }
  }

  private resetSVGAnimation() {
    const circle = document.querySelector('.circle') as SVGCircleElement;
    if (circle) {
      circle.style.animation = 'none';
      circle.style.animation = `circletimer ${this.MaxTime}s linear forwards`;
    }
  }
}
