import { AuthenService } from '@/core/service/authen.service';
import { SpinnerComponent } from '@/layout/component/app.spiner';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';

let lang = localStorage.getItem('lang');
@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterModule, SpinnerComponent, ToastModule, MessageModule],
    template: `<p-toast></p-toast><app-spinner></app-spinner>
    <router-outlet></router-outlet>`
})
export class AppComponent {
    constructor(private translate: TranslateService, private auth: AuthenService, private messageService: MessageService) {
        this.translate.setDefaultLang(lang || 'vn');
        this.translate.use(lang || 'vn');
    }

    ngOnInit() {
        // Thiết lập tự động đăng xuất khi token hết hạn
        this.auth.setupAutoLogout(() => {
            this.messageService.add({
                severity: 'warn',
                summary: this.translate.instant('login.notification'),
                detail: this.translate.instant('login.checktoken')
            });

            setTimeout(() => {
                this.auth.logOutAuto();
            }, 2000);
        });
    }
}
