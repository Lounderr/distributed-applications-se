import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LoadingService {
    private loadingSubject = new BehaviorSubject<boolean>(false);
    private activeRequests = 0;

    loading$: Observable<boolean> = this.loadingSubject.asObservable();

    constructor(private ngZone: NgZone) {}

    show(): void {
        this.ngZone.run(() => {
            this.activeRequests++;
            if (this.activeRequests === 1) {
                this.loadingSubject.next(true);
            }
        });
    }

    hide(): void {
        this.ngZone.run(() => {
            this.activeRequests = Math.max(0, this.activeRequests - 1);
            if (this.activeRequests === 0) {
                this.loadingSubject.next(false);
            }
        });
    }

    reset(): void {
        this.ngZone.run(() => {
            this.activeRequests = 0;
            this.loadingSubject.next(false);
        });
    }
} 