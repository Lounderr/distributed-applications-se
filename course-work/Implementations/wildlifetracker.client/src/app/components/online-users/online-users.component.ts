import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { OnlineUsersService } from '../../services/online-users.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
    selector: 'app-online-users',
    standalone: true,
    imports: [CommonModule, MatListModule, MatIconModule],
    template: `
        <div class="online-users-container">
            <h6>Online Users</h6>
            <mat-nav-list>
                <div mat-list-item *ngFor="let userEmail of onlineUserEmails">
                    <span>{{ userEmail }}</span>
                </div>
                <div mat-list-item *ngIf="onlineUserEmails.length === 0">
                    <span class="no-users">No users online</span>
                </div>
            </mat-nav-list>
        </div>
    `,
    styles: [`
        .online-users-container {
            padding: 16px;
        }
        h3 {
            margin: 0 0 16px 0;
            color: #666;
        }
        .online-indicator {
            color: #4CAF50;
            font-size: 12px;
            margin-right: 8px;
        }
        mat-nav-list {
            padding-top: 0;
        }
        .no-users {
            color: #666;
            font-style: italic;
        }
    `]
})
export class OnlineUsersComponent implements OnInit, OnDestroy {
    onlineUserEmails: string[] = [];
    private subscription: Subscription = new Subscription();

    constructor(private onlineUsersService: OnlineUsersService) {}

    ngOnInit(): void {
        // Poll for online users every 30 seconds
        this.subscription = interval(30000)
            .pipe(
                switchMap(() => this.onlineUsersService.getOnlineUsers())
            )
            .subscribe({
                next: (users) => {
                    this.onlineUserEmails = users;
                },
                error: (error) => {
                    console.error('Error fetching online users:', error);
                }
            });

        // Initial fetch
        this.onlineUsersService.getOnlineUsers().subscribe({
            next: (users) => {
                this.onlineUserEmails = users;
            },
            error: (error) => {
                console.error('Error fetching online users:', error);
            }
        });
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
} 