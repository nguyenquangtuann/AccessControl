import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AppMenuitem } from './app.menuitem';
import { DataService } from '@/core/service/data.service';
import { SystemConstants } from '@/core/common/system.constants';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
export interface lstMenuItem {
    label?: string;
    icon?: string;
    routerLink?: any;
    items?: MenuItem[];
}
@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, RouterModule, AppMenuitem, TranslateModule],
    template: `<ul class="layout-menu">
        <ng-container *ngFor="let item of model; let i = index">
            <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true"></li>
            <li *ngIf="item.separator" class="menu-separator"></li>
        </ng-container>
    </ul> `
})
export class AppMenu implements OnInit {
    model: MenuItem[] = [];
    userInfo: any;
    constructor(private dataService: DataService, private translate: TranslateService) { }
    ngOnInit() {
        this.getUser();
        this.getlistUser();
        this.loadMenuUsers();
    }

    loadMenuUsers() {
        const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
        this.dataService.get('AppRole/gettreeviewbyuser?userId=' + user.id).subscribe((data: any) => {
            this.model = data.map((m: any) => this.mapToMenuItem(m));
            // vứt Dashboard lên đầu tiên
            this.model.unshift({
                label: 'menu.dashboard',
                items: [
                    { label: 'menu.dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/dashboard'] }
                ]
            });
        });
    }

    getlistUser() {
        this.dataService.get('AppUser/getall').subscribe((data: any) => {
            localStorage.removeItem(SystemConstants.USERS_PIPE);
            localStorage.setItem(SystemConstants.USERS_PIPE, JSON.stringify(data));
        });
    }

    getUser() {
        let current = localStorage.getItem(SystemConstants.CURRENT_USER);
        this.userInfo = JSON.parse(current!);
    }

    mapToMenuItem(apiMenu: any): lstMenuItem {
        return {
            label: apiMenu.menuName,
            icon: apiMenu.icon ? 'pi pi-fw ' + apiMenu.icon : undefined,
            routerLink: apiMenu.activeLink || undefined,
            items: apiMenu.childrens ? apiMenu.childrens.map((child: any) => this.mapToMenuItem(child)) : undefined
        };
    }
}
