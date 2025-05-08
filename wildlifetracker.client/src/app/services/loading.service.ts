import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LoadingService {
    private loadingSubject = new BehaviorSubject<boolean>(false);
    private activeRequests = 0;

    loading$: Observable<boolean> = this.loadingSubject.asObservable();

    show(): void {
        this.activeRequests++;
        this.loadingSubject.next(true);
    }

    hide(): void {
        this.activeRequests--;
        if (this.activeRequests === 0) {
            this.loadingSubject.next(false);
        }
    }

    reset(): void {
        this.activeRequests = 0;
        this.loadingSubject.next(false);
    }
} 