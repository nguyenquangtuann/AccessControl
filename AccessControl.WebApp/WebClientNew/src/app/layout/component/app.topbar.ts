import { Component, OnInit } from '@angular/core';
import { MenuItem, MessageService } from 'primeng/api';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { AppConfigurator } from './app.configurator';
import { LayoutService } from '../service/layout.service';
import { Popover, PopoverModule } from 'primeng/popover';
import { SystemConstants } from '@/core/common/system.constants';
import { ButtonModule } from 'primeng/button';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DataService } from '@/core/service/data.service';
import { MessageModule } from 'primeng/message';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AppSpinerService } from '../service/spiner.service';
import { ToastModule } from 'primeng/toast';
import { PasswordModule } from 'primeng/password';
import { UserRoles } from '@/core/common/userRole.pipe';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthenService } from '@/core/service/authen.service';

@Component({
    selector: 'app-topbar',
    standalone: true,
    imports: [RouterModule, CommonModule, StyleClassModule, AppConfigurator, PopoverModule, ButtonModule, MessageModule, DialogModule, InputTextModule, ReactiveFormsModule, PasswordModule, ToastModule, UserRoles, TranslateModule],
    providers: [MessageService],
    template: ` <div class="layout-topbar">
        <div class="layout-topbar-logo-container">
            <button class="layout-menu-button layout-topbar-action" (click)="layoutService.onMenuToggle()">
                <i class="pi pi-bars"></i>
            </button>
            <a class="layout-topbar-logo" routerLink="/dashboard">
                <svg viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg" role="img" aria-label="Clock Logo">
                    <defs>
                        <style>
                        .primary { stroke: var(--primary-color, #0066ff); stroke-width: 3; stroke-linecap:round; stroke-linejoin:round; fill: none; }
                        .fill { fill: var(--primary-color, #0066ff); }
                        </style>
                    </defs>
                    <circle cx="32" cy="32" r="26" class="primary"/>
                    <line x1="32" y1="8" x2="32" y2="12" class="primary"/>
                    <line x1="32" y1="52" x2="32" y2="56" class="primary"/>
                    <line x1="8" y1="32" x2="12" y2="32" class="primary"/>
                    <line x1="52" y1="32" x2="56" y2="32" class="primary"/>
                    <line x1="32" y1="32" x2="32" y2="18" class="primary"/>
                    <line x1="32" y1="32" x2="44" y2="32" class="primary"/>
                    <circle cx="32" cy="32" r="3" class="fill"/>
                </svg>
                <span>TIME ATTENDANCE</span>
            </a>
        </div>

        <div class="layout-topbar-actions">
            <button class="layout-topbar-menu-button layout-topbar-action" pStyleClass="@next" enterFromClass="hidden" enterActiveClass="animate-scalein" leaveToClass="hidden" leaveActiveClass="animate-fadeout" [hideOnOutsideClick]="true">
                <i class="pi pi-ellipsis-v"></i>
            </button>

            <div class="layout-topbar-menu hidden lg:block">
                <div class="layout-topbar-menu-content">
                    <button type="button" class="layout-topbar-action" (click)="toggleDarkMode()">
                        <i [ngClass]="{ 'pi ': true, 'pi-moon': layoutService.isDarkTheme(), 'pi-sun': !layoutService.isDarkTheme() }"></i>
                        <span>{{'topbar.topic' | translate}}</span>
                    </button>

                    <button type="button" class="layout-topbar-action" (click)="toggleLaguage(op1, $event)">
                        <i class="pi pi-globe"></i>
                        <span>{{'topbar.language' | translate}}</span>
                    </button>
                    <p-popover #op1>
                        <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1px 15px; align-items: center; justify-content: center;">
                            <img style="width: 36px; cursor: pointer" src="assets/flags/vn.png" (click)="changeLang('vn'); op1.hide()"/>
                            <img style="width: 36px; cursor: pointer" src="assets/flags/en.png" (click)="changeLang('en'); op1.hide()"/>
                            <img style="width: 36px; cursor: pointer" src="assets/flags/cn.png" (click)="changeLang('cn'); op1.hide()"/>
                        </div>
                    </p-popover>

                    <div class="relative">
                        <button class="layout-topbar-action layout-topbar-action-highlight" pStyleClass="@next" enterFromClass="hidden" enterActiveClass="animate-scalein" leaveToClass="hidden" leaveActiveClass="animate-fadeout" [hideOnOutsideClick]="true">
                            <i class="pi pi-palette"></i>
                            <span>{{'topbar.color' | translate}}</span>
                        </button>
                        <app-configurator />
                    </div>

                    <button type="button" class="layout-topbar-action" (click)="toggleForm(op2, $event)">
                        <i class="pi pi-user"></i>
                        <span>{{'topbar.profile' | translate}}</span>
                    </button>
                    <p-popover #op2 [style]="{ width: '275px' }">
                        <div>
                            <h5>{{'topbar.userprofile' | translate}}</h5>
                            <div style="display: flex; gap: 15px; align-items: center; margin-bottom: 16px; margin-top: 20px;">
                                <img src="assets/images/user-none.png" *ngIf="imageSrc == null || imageSrc == ''" style="width: 90px; height: 90px; border-radius: 50%;"/>
                                <img src="{{'data:image/jpg;base64,' + imageSrc}}" *ngIf="imageSrc != null && imageSrc != ''" style="width: 90px; height: 90px; border-radius: 50%;"/>
                                <div class="info">
                                    <h6 style="margin: 0; font-size: 13px; font-weight: 500;">{{userInfo.fullName}}</h6>
                                    <span style="font-size: 13px; line-height: 2;">{{userInfo.phone}}</span>
                                    <div style="font-size: 12px;">{{userInfo.email}}</div>
                                </div>
                            </div>
                            <div style="display: flex;gap: 14px;align-items: center;justify-content: center;padding-top:10px">
                                <p-button *ngIf="'ChangePassword' | userRoles" label="{{'topbar.changepassword' | translate}}" (click)="openDialog()"></p-button>
                                <p-button label="{{'topbar.logout' | translate}}" (click)="logOut()"></p-button>
                            </div>
                        </div>
                    </p-popover>
                </div>
            </div>
        </div>
    </div>
    
    <form [formGroup]="form" (ngSubmit)="change()">
        <p-dialog [(visible)]="dialog" [style]="{ width: '450px','max-height': '800px' }" [header]="title" [modal]="true"
            [closable]="false">
            <div class="flex flex-col gap-6">
                <div>
                    <label for="passwordOld" class="block font-bold mb-3">{{'topbar.oldpassword' | translate}}</label>
                    <p-password id="passwordOld" formControlName="passwordOld" [toggleMask]="true" [feedback]="false" autofocus fluid
                        required></p-password>
                    <small class="text-red-500"
                        *ngIf="form.get('passwordOld')?.hasError('required') && form.get('passwordOld')?.touched">
                        {{'topbar.oldpasswordrequired' | translate}}
                    </small>
                </div>

                <div>
                    <label for="passwordNew" class="block font-bold mb-3">{{'topbar.newpassword' | translate}}</label>
                    <p-password id="passwordNew" formControlName="passwordNew" [toggleMask]="true" [feedback]="false" autofocus fluid
                        required></p-password>
                    <small class="text-red-500"
                        *ngIf="form.get('passwordNew')?.hasError('required') && form.get('passwordNew')?.touched">
                        {{'topbar.newpasswordrequired' | translate}}
                    </small>
                </div>
                
                <div>
                    <label for="passwordNewRepeat" class="block font-bold mb-3">{{'topbar.confirmpassword' | translate}}</label>
                    <p-password id="passwordNewRepeat" formControlName="passwordNewRepeat" [toggleMask]="true" [feedback]="false" autofocus fluid
                        required></p-password>
                    <small class="text-red-500"
                        *ngIf="form.get('passwordNewRepeat')?.hasError('required') && form.get('passwordNewRepeat')?.touched">
                        {{'topbar.confirmpasswordrequired' | translate}}
                    </small>
                </div>
            </div>
            <ng-template pTemplate="footer">
                <p-button label="{{'topbar.close' | translate}}" icon="pi pi-times" text (click)="hideDialog()" />
                <p-button label="{{'topbar.save' | translate}}" type="submit" icon="pi pi-check" [disabled]="form.invalid" />
            </ng-template>
        </p-dialog>
    </form>
    <p-toast />
    `
})
export class AppTopbar implements OnInit {
    items!: MenuItem[];
    userInfo: any;
    form!: FormGroup;
    dialog = false;
    title: string = '';
    imageSrc: string = '';

    constructor(private router: Router, private formBuilder: FormBuilder, private dataService: DataService, public layoutService: LayoutService, private messageService: MessageService, private spiner: AppSpinerService, private translate: TranslateService, private auth: AuthenService) {
        this.form = this.formBuilder.group({
            passwordOld: ['', Validators.compose([Validators.required])],
            passwordNew: ['', Validators.compose([Validators.required])],
            passwordNewRepeat: ['', Validators.compose([Validators.required])],
        })
    }

    ngOnInit(): void {
        this.getUser();
    }

    // đổi ngôn ngữ
    changeLang(lang: string) {
        this.translate.use(lang);
        localStorage.setItem('lang', lang); // lưu lại
    }

    toggleDarkMode() {
        this.layoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
    }

    toggleForm(op: Popover, event: any) {
        op.toggle(event);
    }

    toggleLaguage(op: Popover, event: any) {
        op.toggle(event);
    }

    getUser() {
        let current = localStorage.getItem(SystemConstants.CURRENT_USER);
        this.userInfo = JSON.parse(current!);
        this.getImageByEmId(this.userInfo.emId);
    }

    getImageByEmId(emId: any) {
        this.dataService.get('Employee/getbyid?id=' + emId).subscribe((respon: any) => {
            this.imageSrc = respon.emImage;
        }, (err: any) => {
            this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('topbar.error'),
                detail: this.translate.instant('topbar.detailgetem') + err.statusText
            });
        })
    }

    logOut() {
        this.auth.logOut();
    }

    openDialog() {
        this.dialog = true;
        this.title = this.translate.instant('topbar.changepassword');
    }

    hideDialog() {
        this.dialog = false;
        this.form.reset();
    }

    change() {
        const oldpass = this.form.controls['passwordOld'].value;
        const newpass = this.form.controls['passwordNew'].value;
        const newPassRepeat = this.form.controls['passwordNewRepeat'].value;
        if (!this.form.invalid) {
            this.spiner.show();
            // check mật khẩu cũ có đúng mật khẩu đang đăng nhập không
            this.dataService.get('AccountLogin/Checkpassword?userName=' + JSON.parse(localStorage['CURRENT_USER']).userName + '&pass=' + oldpass)
                .subscribe((respon: any) => {
                    var check = respon;
                    if (check == false) {
                        this.messageService.add({
                            severity: 'warn',
                            summary: this.translate.instant('topbar.notification'),
                            detail: this.translate.instant('topbar.oldpasswordincorrect')
                        });
                    }
                    this.spiner.hide();
                    return;
                })
            //end check mật khẩu
            if (oldpass == newpass) {
                this.messageService.add({
                    severity: 'warn',
                    summary: this.translate.instant('topbar.notification'),
                    detail: this.translate.instant('topbar.newpasswordsameold')
                });
                this.spiner.hide();
                return;
            }
            if (newpass != newPassRepeat) {
                this.messageService.add({
                    severity: 'warn',
                    summary: this.translate.instant('topbar.notification'),
                    detail: this.translate.instant('topbar.newpasswordnotmatch')
                });
                this.spiner.hide();
                return;
            }
            //Đổi mật khẩu
            let param = {
                id: JSON.parse(localStorage['CURRENT_USER']).id,
                passwordHash: newpass
            }
            this.dataService.put('AppUser/changepassword', param).subscribe((respon: any) => {
                this.messageService.add({
                    severity: 'success',
                    summary: this.translate.instant('topbar.notification'),
                    detail: this.translate.instant('topbar.changepasswordsuccess')
                });
                this.spiner.hide();
                this.hideDialog();
                this.router.navigate(['/login']);
            }, (err: any) => {
                this.messageService.add({
                    severity: 'error',
                    summary: this.translate.instant('topbar.error'),
                    detail: err.error.message
                });
                this.spiner.hide();
            })
            //end đổi mk
        }
    }
}
