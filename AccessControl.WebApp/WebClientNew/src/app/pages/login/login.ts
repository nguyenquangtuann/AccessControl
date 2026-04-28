import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { AppFloatingConfigurator } from '../../layout/component/app.floatingconfigurator';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { AuthenService } from '@/core/service/authen.service';
import { SystemConstants } from '@/core/common/system.constants';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [ButtonModule, CheckboxModule, InputTextModule, PasswordModule, ReactiveFormsModule, RouterModule, RippleModule, AppFloatingConfigurator, CommonModule, MessageModule, ToastModule, TranslateModule],
    templateUrl: './login.html',
})
export class Login implements OnInit {
    loginForm!: FormGroup;

    constructor(private formBuilder: FormBuilder, private router: Router, private authenService: AuthenService, private message: MessageService, private spiner: AppSpinerService, private translate: TranslateService) {
        this.loginForm = this.formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required],
            checked: false
        });
    }

    ngOnInit(): void {
        const check = JSON.parse(localStorage.getItem('checked')!);
        const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
        if (check == true && user != null) {
            this.loginForm.controls['username'].setValue(user.userName);
            this.loginForm.controls['checked'].setValue(true);
        }
    }

    login() {
        if (this.loginForm.valid) {
            this.spiner.show();
            if (this.loginForm.controls['checked'].value == true) {
                this.authenService.login(this.loginForm.value).subscribe(data => {
                    this.spiner.hide();
                    this.router.navigate(['/dashboard']);
                    localStorage.setItem('checked', 'true');
                    // Thiết lập tự động đăng xuất khi token hết hạn
                    this.authenService.setupAutoLogout(() => {
                        this.message.add({
                            severity: 'warn',
                            summary: this.translate.instant('login.notification'),
                            detail: this.translate.instant('login.checktoken')
                        });

                        setTimeout(() => {
                            this.authenService.logOutAuto();
                        }, 2000); // Đợi 2 giây trước khi tự động đăng xuất để người dùng có thể đọc thông báo
                    });
                }, err => {
                    this.spiner.hide();
                    if (err.status === 401) {
                        this.message.add({ severity: 'warn', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.invalidcredentials') });
                    }
                    else if (err.status === 400) {
                        this.message.add({ severity: 'warn', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.usernameandpasswordrequired') });
                    }
                    else {
                        this.message.add({ severity: 'error', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.servererror') });
                    }
                });
            }
            else {
                this.authenService.login(this.loginForm.value).subscribe(data => {
                    this.spiner.hide();
                    this.router.navigate(['/dashboard']);
                    localStorage.setItem('checked', 'false');
                    // Thiết lập tự động đăng xuất khi token hết hạn
                    this.authenService.setupAutoLogout(() => {
                        this.message.add({
                            severity: 'warn',
                            summary: this.translate.instant('login.notification'),
                            detail: this.translate.instant('login.checktoken')
                        });

                        setTimeout(() => {
                            this.authenService.logOutAuto();
                        }, 2000);
                    });
                }, err => {
                    this.spiner.hide();
                    if (err.status === 401) {
                        this.message.add({ severity: 'warn', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.invalidcredentials') });
                    }
                    else if (err.status === 400) {
                        this.message.add({ severity: 'warn', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.usernameandpasswordrequired') });
                    }
                    else {
                        this.message.add({ severity: 'error', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.servererror') });
                    }
                });
            }
        }
        else {
            this.message.add({ severity: 'warn', summary: this.translate.instant('login.notification'), detail: this.translate.instant('login.usernameandpasswordrequired') });
        }
    }
}
