import { Component, HostBinding, Input, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { RippleModule } from 'primeng/ripple';
import { MenuItem } from 'primeng/api';
import { LayoutService } from '../service/layout.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: '[app-menuitem]',
    imports: [CommonModule, RouterModule, RippleModule, TranslateModule],
    template: `
        <ng-container>
            <div *ngIf="root && item.visible !== false" class="layout-menuitem-root-text" (click)="itemClick($event)" style="cursor: pointer; display: flex; align-items: center">
                <i class="pi pi-fw pi-angle-down" *ngIf="item.label !== 'menu.dashboard'" [style.transform]="active ? 'rotate(0deg)' : 'rotate(-90deg)'" 
                    style="transition: transform 0.2s;margin-right:5px"></i>
                {{ item.label | translate }}
            </div>
            <a *ngIf="(!item.routerLink || item.items) && item.visible !== false" [attr.href]="item.url" (click)="itemClick($event)" [ngClass]="item.styleClass" [attr.target]="item.target" tabindex="0" pRipple>
                <i [ngClass]="item.icon" class="layout-menuitem-icon"></i>
                <span class="layout-menuitem-text">{{ item.label | translate }}</span>
                <i class="pi pi-fw pi-angle-down layout-submenu-toggler" *ngIf="item.items"></i>
            </a>
            <a
                *ngIf="item.routerLink && !item.items && item.visible !== false"
                (click)="itemClick($event)"
                [ngClass]="item.styleClass"
                [routerLink]="item.routerLink"
                routerLinkActive="active-route"
                [routerLinkActiveOptions]="item.routerLinkActiveOptions || { paths: 'exact', queryParams: 'ignored', matrixParams: 'ignored', fragment: 'ignored' }"
                [fragment]="item.fragment"
                [queryParamsHandling]="item.queryParamsHandling"
                [preserveFragment]="item.preserveFragment"
                [skipLocationChange]="item.skipLocationChange"
                [replaceUrl]="item.replaceUrl"
                [state]="item.state"
                [queryParams]="item.queryParams"
                [attr.target]="item.target"
                tabindex="0"
                pRipple
            >
                <i [ngClass]="item.icon" class="layout-menuitem-icon"></i>
                <span class="layout-menuitem-text">{{ item.label | translate}}</span>
                <i class="pi pi-fw pi-angle-down layout-submenu-toggler" *ngIf="item.items"></i>
            </a>

            <ul *ngIf="item.items && item.visible !== false" [@children]="submenuAnimation">
        <ng-template ngFor let-child let-i="index" [ngForOf]="item.items">
            <li app-menuitem [item]="child" [index]="i" [parentKey]="key" [class]="child['badgeClass']"></li>
        </ng-template>
    </ul>
        </ng-container>
    `,
    animations: [
        trigger('children', [
            state('collapsed', style({
                height: '0',
                overflow: 'hidden'
            })),
            state('expanded', style({
                height: '*'
            })),
            transition('collapsed <=> expanded', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
        ])
    ],
    providers: [LayoutService]
})
export class AppMenuitem implements OnInit, OnDestroy {
    @Input() item!: MenuItem;
    @Input() index!: number;
    @Input() @HostBinding('class.layout-root-menuitem') root!: boolean;
    @Input() parentKey!: string;

    active = false;
    key: string = '';
    menuSourceSubscription: Subscription;
    menuResetSubscription: Subscription;

    constructor(public router: Router, private layoutService: LayoutService, private translate: TranslateService) {
        this.menuSourceSubscription = this.layoutService.menuSource$.subscribe((value) => {
            Promise.resolve(null).then(() => {
                if (this.item.label === 'menu.dashboard') {
                    this.active = true;
                    return;
                }

                if (value.routeEvent) {
                    this.active = value.key === this.key || value.key.startsWith(this.key + '-') ? true : false;
                } else {
                    if (value.key !== this.key && !value.key.startsWith(this.key + '-')) {
                        this.active = false;
                    }
                }
            });
        });

        this.menuResetSubscription = this.layoutService.resetSource$.subscribe(() => {
            if (this.item.label !== 'menu.dashboard') {
                this.active = false;
            }
        });

        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            if (this.item.routerLink) {
                this.updateActiveStateFromRoute();
            }
        });
    }

    ngOnInit() {
        this.key = this.parentKey ? this.parentKey + '-' + this.index : String(this.index);

        if (this.item.label === 'menu.dashboard') {
            this.active = true;
        }

        if (this.item.routerLink) {
            this.updateActiveStateFromRoute();
        }
    }

    updateActiveStateFromRoute() {
        let activeRoute = this.router.isActive(this.item.routerLink[0], { paths: 'exact', queryParams: 'ignored', matrixParams: 'ignored', fragment: 'ignored' });

        if (activeRoute) {
            this.layoutService.onMenuStateChange({ key: this.key, routeEvent: true });
        }
    }

    itemClick(event: Event) {
        if (this.item.disabled) {
            event.preventDefault();
            return;
        }

        if (this.item.items) {
            if (this.item.label === 'menu.dashboard') {
                this.active = true;
            } else {
                this.active = !this.active;
            }
        }

        this.layoutService.onMenuStateChange({ key: this.key });
    }

    get submenuAnimation() {
        if (this.item.label === 'menu.dashboard') return 'expanded';
        return this.active ? 'expanded' : 'collapsed';
    }

    @HostBinding('class.active-menuitem')
    get activeClass() {
        return this.active;
    }

    ngOnDestroy() {
        if (this.menuSourceSubscription) this.menuSourceSubscription.unsubscribe();
        if (this.menuResetSubscription) this.menuResetSubscription.unsubscribe();
    }
}
