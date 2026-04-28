import { Component, computed, inject, input } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { StyleClassModule } from 'primeng/styleclass';
import { AppConfigurator } from './app.configurator';
import { LayoutService } from '../service/layout.service';
import { CommonModule } from "@angular/common";
import { Popover, PopoverModule } from 'primeng/popover';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-floating-configurator',
    imports: [CommonModule, ButtonModule, StyleClassModule, AppConfigurator, PopoverModule, TranslateModule],
    template: `
        <div class="flex gap-4 top-8 right-8" [ngClass]="{'fixed':float()}">
            <p-button type="button" (onClick)="toggleDarkMode()" [rounded]="true" [icon]="isDarkTheme() ? 'pi pi-moon' : 'pi pi-sun'" severity="secondary" />
            <p-button icon="pi pi-globe" type="button" [rounded]="true" (onClick)="toggleLaguage(op1, $event)" severity="secondary" />
            <p-popover #op1>
                <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1px 15px; align-items: center; justify-content: center;">
                    <img style="width: 36px; cursor: pointer" src="assets/flags/vn.png" (click)="changeLang('vn'); op1.hide()"/>
                    <img style="width: 36px; cursor: pointer" src="assets/flags/en.png" (click)="changeLang('en'); op1.hide()"/>
                    <img style="width: 36px; cursor: pointer" src="assets/flags/cn.png" (click)="changeLang('cn'); op1.hide()"/>
                </div>
            </p-popover>
            <div class="relative">
                <p-button icon="pi pi-palette" pStyleClass="@next" enterFromClass="hidden" enterActiveClass="animate-scalein" leaveToClass="hidden" leaveActiveClass="animate-fadeout" [hideOnOutsideClick]="true" type="button" rounded />
                <app-configurator />
            </div>
        </div>
    `
})
export class AppFloatingConfigurator {
    constructor(private translate: TranslateService) { }

    LayoutService = inject(LayoutService);

    float = input<boolean>(true);

    isDarkTheme = computed(() => this.LayoutService.layoutConfig().darkTheme);

    toggleDarkMode() {
        this.LayoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
    }

    // đổi ngôn ngữ
    changeLang(lang: string) {
        this.translate.use(lang);
        localStorage.setItem('lang', lang); // lưu lại
    }

    toggleLaguage(op: Popover, event: any) {
        op.toggle(event);
    }
}
