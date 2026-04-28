import { SystemConstants } from '@/core/common/system.constants';
import { UserRoles } from '@/core/common/userRole.pipe';
import { DataService } from '@/core/service/data.service';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
@Component({
  selector: 'app-regency',
  standalone: true,
  imports: [CommonModule, TableModule, FormsModule, ButtonModule, ToastModule, ToolbarModule, InputTextModule, DialogModule, InputIconModule, IconFieldModule, ConfirmDialogModule, MessageModule, UserRoles, ReactiveFormsModule, CheckboxModule, TagModule, TranslateModule],
  templateUrl: './regency.html',
  providers: [MessageService, ConfirmationService]
})
export class Regency implements OnInit {
  keyword: string = '';
  allRegency: any[] = [];
  page = 0;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  regDialog = false;
  regForm!: FormGroup;
  action: string = '';
  title: string = '';
  selected: any;
  @ViewChild('dt') dt!: Table;

  constructor(private dataService: DataService, private messageService: MessageService, private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService) {
    this.regForm = this.formBuilder.group({
      regId: 0,
      regName: ['', Validators.compose([Validators.required, Validators.maxLength(100)])],
      regDescription: ['', Validators.compose([Validators.maxLength(200)])],
      regStatus: '',
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      deleteBy: '',
      deleteDate: ''
    });
  }

  ngOnInit(): void {
    this.getAllReg();
  }

  getAllReg() {
    this.dataService.get('Regency/getlistpaging?page=' + this.page + '&pageSize=' + this.pageSize + '&keyword=' + this.keyword).subscribe((data: any) => {
      this.allRegency = data.items;
      this.totalRecords = data.totalCount;
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('regency.error'),
        detail: this.translate.instant('regency.detailget')
      });
    })
  }

  openDialog(action: string, item?: any) {
    this.regDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('regency.addtitle');
      this.regForm.controls['regStatus'].setValue(true);
    }
    else {
      this.title = this.translate.instant('regency.edittitle');
      this.regForm.controls['regId'].setValue(item.regId);
      let itemFilter = this.allRegency.filter((x: any) => x.regId == item.regId)[0];
      this.regForm.controls['regName'].setValue(itemFilter.regName);
      this.regForm.controls['regDescription'].setValue(itemFilter.regDescription);
      this.regForm.controls['regStatus'].setValue(itemFilter.regStatus);
      this.regForm.controls['createdDate'].setValue(itemFilter.createdDate);
      this.regForm.controls['createdBy'].setValue(itemFilter.createdBy);
      this.regForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
      this.regForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
      this.regForm.controls['deleteDate'].setValue(itemFilter.deleteDate);
      this.regForm.controls['deleteBy'].setValue(itemFilter.deleteBy);
    }
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('regency.messagedelmultiple'),
      header: this.translate.instant('regency.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('regency.confirm'),
      rejectLabel: this.translate.instant('regency.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [];
        this.selected.forEach((value: any) => {
          let id = value.regId;
          lstId.push(id);
        });
        let regDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Regency/delete', regDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('regency.notification'),
                detail: this.translate.instant('regency.delsuccess')
              });
              this.getAllReg();
              this.selected = [];
              this.spiner.hide();
            },
            (err: any) => {
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('regency.error'),
                detail: err.error.message
              });
              this.spiner.hide();
            }
          );
      }
    });
  }

  delete(reg: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('regency.messagedel') + reg.regName + this.translate.instant('regency.messageconfirmdel'),
      header: this.translate.instant('regency.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('regency.confirm'),
      rejectLabel: this.translate.instant('regency.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [reg.regId];
        let regDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Regency/delete', regDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('regency.notification'),
                detail: this.translate.instant('regency.delsuccess')
              });
              this.getAllReg();
              this.spiner.hide();
            },
            (err: any) => {
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('regency.error'),
                detail: err.error.message
              });
              this.spiner.hide();
            }
          );
      }
    });
  }

  applySearch(event: Event) {
    this.keyword = (event.target as HTMLInputElement).value;
    this.page = 0;
    this.getAllReg();
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows;
    this.pageSize = event.rows;
    this.getAllReg();
  }

  addData() {
    if (this.regForm.invalid) {
      this.regForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    if (this.action == 'create') {
      this.regForm.controls['createdBy'].setValue(user.id);
      let reg = this.regForm.value;
      this.dataService.post('Regency/create', reg).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: 'Thông báo',
            detail: 'Thêm mới thành công'
          });
          this.getAllReg();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Lỗi',
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    } else if (this.action == 'edit') {
      this.regForm.controls['updatedBy'].setValue(user.id);
      let reg = this.regForm.value;
      this.dataService.put('Regency/update', reg).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: 'Thông báo',
            detail: 'Cập nhật thông tin thành công'
          });
          this.getAllReg();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Lỗi',
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
  }

  hideDialog() {
    this.action == '';
    this.regDialog = false;
    this.regForm.reset({
      regId: 0,
      regName: '',
      regDescription: '',
      regStatus: '',
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      deleteBy: '',
      deleteDate: ''
    });
  }
}
