import { Component, Input, OnDestroy, Renderer2, ElementRef, HostListener, isDevMode } from '@angular/core';
import { GameService } from '../../game.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-timer',
  templateUrl: './timer.component.html',
  styleUrls: ['./timer.component.css']
})
export class TimerComponent implements OnDestroy {
  public time: number = 0;
  public MaxTime: number = 0;
  public circumference: number = 0;
  public timerSize: number = 200;
  private interval: ReturnType<typeof setInterval> | null = null;
  private questionStartedSubscription: Subscription;
  private newGameStateSubscription: Subscription;
  private questionEndedSubscription: Subscription;

  constructor(private gs: GameService, private renderer: Renderer2, private el: ElementRef) {
    this.circumference = (this.timerSize / 2 - 12) * 2 * Math.PI;

    this.questionStartedSubscription = gs.questionStarted$.subscribe((question) => this.onNewQuestion(new Date(question.ExpireTime)));
    this.newGameStateSubscription = gs.newGameState$.subscribe((state) => {
      if (state.CurrentQuestion) {
        this.onNewQuestion(new Date(state.CurrentQuestion.ExpireTime));
      }
    });
    this.questionEndedSubscription = gs.questionEnded$.subscribe(() => {
      const circleElement = this.el.nativeElement.querySelector('.circle');
      this.renderer.addClass(circleElement, 'no-transition');
      if (this.interval) {
        clearInterval(this.interval);
      }
      this.time = 0;
      setTimeout(() => {
        const circleElement = this.el.nativeElement.querySelector('.circle');
        this.renderer.removeClass(circleElement, 'no-transition');
      }, 50);
    });
    this.onResize(window.innerWidth);
  }

  ngOnDestroy() {
    if (this.interval) {
      clearInterval(this.interval);
    }
    if (this.questionStartedSubscription) {
      this.questionStartedSubscription.unsubscribe();
    }
    if (this.newGameStateSubscription) {
      this.newGameStateSubscription.unsubscribe();
    }
    if (this.questionEndedSubscription) {
      this.questionEndedSubscription.unsubscribe();
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event | number) {
    const width = typeof event === 'number' ? event : (event.target as Window)?.innerWidth ?? window.innerWidth;
    if (width < 768) {
      this.timerSize = 165;
    } else if (width < 1200) {
      this.timerSize = 200;
    } else {
      this.timerSize = 250;
    }
    this.circumference = (this.timerSize / 2 - 12) * 2 * Math.PI;
  }

  public onNewQuestion(expireTime: Date) {
    const now = new Date();
    const circleElement = this.el.nativeElement.querySelector('.circle');

    this.renderer.addClass(circleElement, 'no-transition');
    this.MaxTime = Math.round((expireTime.getTime() - now.getTime()) / 1000);
    this.time = this.MaxTime + 1;

    setTimeout(() => {
      const circleElement = this.el.nativeElement.querySelector('.circle');
      this.renderer.removeClass(circleElement, 'no-transition');

      if (this.MaxTime > 0) {
        this.startTimer();
      }
    }, 50);
  }

  public startTimer() {
    if (this.interval) {
      clearInterval(this.interval);
    }
    this.time = this.MaxTime;

    this.interval = setInterval(() => {
      if (this.time > 1) {
        this.time--;
      } else {
        this.time = 0;
        if (this.interval) {
          clearInterval(this.interval);
        }
        this.interval = null;
        if (this.gs.isUserHost()) {
          this.gs.EndQuestion();
        }
      }
    }, 1000);
  }
}
