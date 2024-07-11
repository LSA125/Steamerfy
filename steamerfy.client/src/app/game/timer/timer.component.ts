import { Component, Input, OnDestroy, Renderer2, ElementRef, HostListener } from '@angular/core';
import { GameService } from '../../game.service';
import { Subscription, delay } from 'rxjs';

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
  private interval: any;
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
  onResize(event: any) {
    const width = event.target ? event.target.innerWidth : event;
    if (width < 768) {
      this.timerSize = 165; // Smaller size for small screens
    } else if (width < 1200) {
      this.timerSize = 200; // Default size for larger screens
    } else {
      this.timerSize = 250; // Larger size for larger screens
    }
    this.circumference = (this.timerSize / 2 - 12) * 2 * Math.PI;
  }

  public onNewQuestion(expireTime: Date) {
    const now = new Date();
    const circleElement = this.el.nativeElement.querySelector('.circle');

    // Add the no-transition class
    this.renderer.addClass(circleElement, 'no-transition');
    this.MaxTime = Math.round((expireTime.getTime() - now.getTime()) / 1000);
    this.time = this.MaxTime + 1;

    setTimeout(() => {
      const circleElement = this.el.nativeElement.querySelector('.circle');
      this.renderer.removeClass(circleElement, 'no-transition');
      console.log("MaxTime: " + this.MaxTime);

      if (this.MaxTime > 0) {
        this.startTimer();
      }
    },50);

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
        clearInterval(this.interval);
        this.interval = null;
        if (this.gs.isUserHost()) {
          console.log("Ending question");
          this.gs.EndQuestion();
        }
      }
    }, 1000);
  }
}
