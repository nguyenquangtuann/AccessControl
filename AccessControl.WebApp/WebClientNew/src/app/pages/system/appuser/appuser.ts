import { Component, OnInit, ViewChild } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table, TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { RatingModule } from 'primeng/rating';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputNumberModule } from 'primeng/inputnumber';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DataService } from '@/core/service/data.service';
import { SystemConstants } from '@/core/common/system.constants';
import { UserPipe } from '@/core/common/userPipe.pipe';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { UserRoles } from '@/core/common/userRole.pipe';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CheckboxModule } from 'primeng/checkbox';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-appuser',
  standalone: true,
  imports: [
    CommonModule, TableModule, FormsModule, ButtonModule, RippleModule, ToastModule,
    ToolbarModule, RatingModule, InputTextModule, InputNumberModule, DialogModule, TagModule,
    InputIconModule, IconFieldModule, ConfirmDialogModule, UserPipe, ReactiveFormsModule, AutoCompleteModule, MessageModule, PasswordModule, UserRoles, CheckboxModule, TranslateModule
  ],
  templateUrl: './appuser.html',
  providers: [MessageService, ConfirmationService]
})
export class Appuser implements OnInit {
  user: any;
  userId: any;
  keyword: string = '';
  allUsers: any[] = [];
  employees: any[] = [];
  page = 0;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  userDialog = false;
  userForm!: FormGroup;
  filteredEmployees: any[] = [];
  userGroup: any[] = [];
  filteredGroup: any[] = [];
  action: string = '';
  title: string = '';
  selected: any;
  adminId: string = '144d5520-2550-474a-b805-bbd991ad2f71';

  @ViewChild('dt') dt!: Table;

  constructor(
    private dataService: DataService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService
  ) {
    this.userForm = this.formBuilder.group(
      {
        id: '',
        userName: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
        password: ['', Validators.compose([Validators.maxLength(50), Validators.minLength(6)])],
        confirmPassword: ['', Validators.compose([Validators.maxLength(50), Validators.minLength(6)])],
        fullName: '',
        phoneNumber: ['', Validators.compose([Validators.pattern('^(\\+84-?)[0-9]{9}|(086|096|097|098|032|033|034|035|036|037|038|039|088|091|094|083|084|085|081|082|089|090|093|070|079|077|076|078|092|056|058|099|059)[0-9]{7}$'), Validators.minLength(10)])],
        email: ['', Validators.compose([Validators.email, Validators.maxLength(50)])],
        image: '',
        groupId: ['', Validators.compose([Validators.required])],
        createdDate: '',
        createdBy: '',
        updatedBy: '',
        updatedDate: '',
        emId: ['', Validators.compose([Validators.required])],
        status: true
      },
      { validators: this.passwordMatchValidator }
    );
  }

  async ngOnInit(): Promise<void> {
    await this.loadAllUsers();
    await this.getAllGroups();
    await this.getAllEmployees();

    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER) as string);
    this.userId = this.user.id;

    this.handlePasswordChange();
    this.handleEmployeeChange();
  }

  handlePasswordChange() {

    const passwordCtrl = this.userForm.get('password');
    const confirmCtrl = this.userForm.get('confirmPassword');

    passwordCtrl?.valueChanges.subscribe(value => {

      // thêm mới
      if (this.action === 'create') return;

      // sửa, nếu có nhập mật khẩu thì mới validate confirm password, ngược lại không bắt buộc nhập lại mật khẩu
      if (value && value.trim() !== '') {

        confirmCtrl?.setValidators([
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(50)
        ]);

      } else {

        confirmCtrl?.clearValidators();
        confirmCtrl?.setValue('');
      }

      confirmCtrl?.updateValueAndValidity();
    });
  }

  // Khi chọn nhân viên thì tự động lấy ảnh của nhân viên đó gán vào form, nếu chưa có ảnh mới gán, trường hợp đã có ảnh rồi thì giữ nguyên
  handleEmployeeChange() {
    this.userForm.get('emId')?.valueChanges.subscribe(emp => {
      if (!emp) return;
      this.userForm.get('image')?.setValue(emp.emImage);
    });
  }

  passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    if (password && confirmPassword && password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }

  // Chỉ hiển thị trường mật khẩu khi thêm mới hoặc khi chỉnh sửa tài khoản là admin
  showPasswordField(): boolean {
    return this.action === 'create' ||
      (this.action !== 'create' && this.userId === this.adminId);
  }

  // Lấy danh sachs người dùng phân trang
  async loadAllUsers() {
    this.dataService.get('AppUser/getpaging?page=' + this.page + '&pageSize=' + this.pageSize + '&keyword=' + this.keyword).subscribe(
      (data: any) => {
        this.allUsers = data.items;
        this.totalRecords = data.totalCount;
      },
      (err) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('account.error'),
          detail: this.translate.instant('account.detailget')
        });
      }
    );
  }

  async getAllGroups() {
    this.dataService.get('AppGroup/getall').subscribe(
      (data: any) => {
        this.userGroup = data;
      },
      (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('account.error'),
          detail: this.translate.instant('account.detailgetgroup')
        });
      }
    );
  }

  filterGroup(event: any) {
    const query = event.query.toLowerCase();
    this.filteredGroup = this.userGroup.filter(gr =>
      gr.name.toLowerCase().includes(query)
    );
  }

  async getAllEmployees() {
    this.dataService.get('Employee/getall').subscribe(
      (data: any) => {
        this.employees = data;
      },
      (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('account.error'),
          detail: this.translate.instant('account.detailgetemployee')
        });
      }
    );
  }

  filterEmployee(event: any) {
    const query = event.query.toLowerCase();
    this.filteredEmployees = this.employees.filter(emp =>
      emp.emName.toLowerCase().includes(query)
    );
  }

  openDialog(action: string, item?: any) {
    this.userDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('account.addtitle');
      this.userForm.controls['password'].setValidators([Validators.required, Validators.minLength(6), Validators.maxLength(50)]);
      this.userForm.controls['confirmPassword'].setValidators([Validators.required, Validators.minLength(6), Validators.maxLength(50)]);
      this.userForm.controls['password'].updateValueAndValidity();
      this.userForm.controls['confirmPassword'].updateValueAndValidity();
    }
    else {
      this.title = this.translate.instant('account.edittitle');
      this.validatorsEditUser();
      this.userForm.controls['id'].setValue(item.id);
      let itemFilter = this.allUsers.filter(
        (x: any) => x.id == item.id
      )[0];
      this.userForm.controls['userName'].setValue(itemFilter.userName);
      this.userForm.controls['fullName'].setValue(itemFilter.fullName);
      this.userForm.controls['email'].setValue(itemFilter.email);
      this.userForm.controls['phoneNumber'].setValue(itemFilter.phoneNumber);
      this.userForm.controls['createdDate'].setValue(itemFilter.createdDate);
      this.userForm.controls['createdBy'].setValue(itemFilter.createdBy);
      this.userForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
      this.userForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
      this.userForm.controls['status'].setValue(itemFilter.status);
      this.userForm.controls['groupId'].setValue(
        this.userGroup.filter((g: any) => g.id == itemFilter.groupId)[0]
      );

      this.userForm.controls['emId'].setValue(
        this.employees.filter((e: any) => e.emId == itemFilter.emId)[0]
      );
    }
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('account.messagedelmultiple'),
      header: this.translate.instant('account.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('account.confirm'),
      rejectLabel: this.translate.instant('account.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [];
        this.selected.forEach((value: any) => {
          let id = value.id;
          lstId.push(id);
        });
        if (!lstId.includes(this.userId)) {
          let userDelete = {
            lstId: JSON.stringify(lstId),
            userId: this.userId
          }
          this.dataService
            .deleteData('AppUser/delete', userDelete).subscribe(
              (response) => {
                this.messageService.add({
                  severity: 'success',
                  summary: this.translate.instant('account.notification'),
                  detail: this.translate.instant('account.deletesuccess')
                });
                this.loadAllUsers();
                this.selected = [];
                this.spiner.hide();
              },
              (err: any) => {
                this.messageService.add({
                  severity: 'error',
                  summary: this.translate.instant('account.error'),
                  detail: err.error.message
                });
                this.spiner.hide();
              }
            );
        }
        else {
          this.messageService.add({
            severity: 'warn',
            summary: this.translate.instant('account.notification'),
            detail: this.translate.instant('account.delerror')
          });
          this.spiner.hide();
          return;
        }
      }
    });
  }

  delete(user: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('account.messagedel') + user.userName + this.translate.instant('account.messageconfirmdel'),
      header: this.translate.instant('account.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('account.confirm'),
      rejectLabel: this.translate.instant('account.reject'),
      accept: () => {
        this.spiner.show();
        if (user.id != this.userId) {
          let lstId: any[] = [user.id];
          let userDelete = {
            lstId: JSON.stringify(lstId),
            userId: this.userId
          }
          this.dataService
            .deleteData('AppUser/delete', userDelete).subscribe(
              (response) => {
                this.messageService.add({
                  severity: 'success',
                  summary: this.translate.instant('account.notification'),
                  detail: this.translate.instant('account.delsuccess')
                });
                this.loadAllUsers();
                this.spiner.hide();
              },
              (err: any) => {
                this.messageService.add({
                  severity: 'error',
                  summary: this.translate.instant('account.error'),
                  detail: err.error.message
                });
                this.spiner.hide();
              }
            );
        }
        else {
          this.messageService.add({
            severity: 'warn',
            summary: this.translate.instant('account.notification'),
            detail: this.translate.instant('account.delerror')
          });
          this.spiner.hide();
          return;
        }
      }
    });
  }

  applySearch(event: Event) {
    this.keyword = (event.target as HTMLInputElement).value;
    this.page = 0;
    this.loadAllUsers();
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows;
    this.pageSize = event.rows;
    this.loadAllUsers();
  }

  addData() {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    if (this.action == 'create') {
      let userData = {
        userName: this.userForm.controls['userName'].value,
        fullName: this.employees.filter((x) => x.emId == this.userForm.controls['emId'].value.emId)[0].emName,
        passwordHash: this.userForm.controls['password'].value,
        phoneNumber: this.userForm.controls['phoneNumber'].value,
        email: this.userForm.controls['email'].value,
        image: null,
        groupId: this.userForm.controls['groupId'].value.id,
        createdBy: this.userId,
        emId: this.userForm.controls['emId'].value.emId,
        status: this.userForm.controls['status'].value
      };
      this.dataService.post('AppUser/create', userData).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('account.notification'),
            detail: this.translate.instant('account.addsuccess')
          });
          this.loadAllUsers();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('account.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    } else if (this.action == 'edit') {
      let userData = {
        userName: this.userForm.controls['userName'].value,
        fullName: this.employees.filter((x) => x.emId == this.userForm.controls['emId'].value.emId)[0].emName,
        passwordHash: this.userForm.controls['password'].value,
        phoneNumber: this.userForm.controls['phoneNumber'].value,
        email: this.userForm.controls['email'].value,
        image: null,
        id: this.userForm.controls['id'].value,
        groupId: this.userForm.controls['groupId'].value.id,
        updatedBy: this.userId,
        createdDate: this.userForm.controls['createdDate'].value,
        createdBy: this.userForm.controls['createdBy'].value,
        emId: this.userForm.controls['emId'].value.emId,
        status: this.userForm.controls['status'].value
      };
      this.dataService.put('AppUser/update', userData).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('account.notification'),
            detail: this.translate.instant('account.updatesuccess')
          });
          this.loadAllUsers();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('account.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
  }

  hideDialog() {
    this.action == '';
    this.userDialog = false;
    this.userForm.reset();
  }

  // Preview image before upload
  imagePreview(e: any) {
    const file = (e.target as HTMLInputElement).files![0];
    const reader = new FileReader();
    reader.onload = () => {
      let base64String = reader.result as string;
      let img = base64String.split('base64,')[1];
      this.userForm.controls['image'].setValue(img);
    };
    reader.readAsDataURL(file);
  }

  clearImage() {
    this.userForm.controls['image'].setValue('');
  }

  validatorsEditUser() {
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
    this.userForm.get('confirmPassword')?.clearValidators();
    this.userForm.get('confirmPassword')?.updateValueAndValidity();
  }
}
