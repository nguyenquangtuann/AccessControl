import { SystemConstants } from '@/core/common/system.constants';
import { UserRoles } from '@/core/common/userRole.pipe';
import { DataService } from '@/core/service/data.service';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CommonModule } from '@angular/common';
import { Component, ViewChild, viewChild, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ConfirmationService, MessageService } from 'primeng/api';
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
  selector: 'app-department',
  standalone: true,
  imports: [CommonModule, TableModule, FormsModule, ButtonModule, ToastModule, ToolbarModule, InputTextModule, DialogModule, InputIconModule, IconFieldModule, ConfirmDialogModule, MessageModule, UserRoles, ReactiveFormsModule, CheckboxModule, TagModule, TranslateModule],
  templateUrl: './department.html',
  providers: [MessageService, ConfirmationService]
})
export class Department implements OnInit {
  keyword: string = '';
  allDepartment: any[] = [];
  page = 0;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  depDialog = false;
  depForm!: FormGroup;
  action: string = '';
  title: string = '';
  selected: any;
  @ViewChild('dt') dt!: Table;

  constructor(private dataService: DataService, private messageService: MessageService, private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService) {
    this.depForm = this.formBuilder.group({
      depId: 0,
      depName: ['', Validators.compose([Validators.required, Validators.maxLength(100)])],
      depDescription: ['', Validators.compose([Validators.maxLength(200)])],
      depStatus: '',
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      deleteBy: '',
      deleteDate: ''
    });
  }

  ngOnInit(): void {
    this.getAllDep();
  }

  getAllDep() {
    this.dataService.get('Department/getlistpaging?page=' + this.page + '&pageSize=' + this.pageSize + '&keyword=' + this.keyword).subscribe((data: any) => {
      this.allDepartment = data.items;
      this.totalRecords = data.totalCount;
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('department.error'),
        detail: this.translate.instant('department.detailget')
      });
    })
  }

  openDialog(action: string, item?: any) {
    this.depDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('department.addtitle');
      this.depForm.controls['depStatus'].setValue(true);
    }
    else {
      this.title = this.translate.instant('department.edittitle');
      this.depForm.controls['depId'].setValue(item.depId);
      let itemFilter = this.allDepartment.filter((x: any) => x.depId == item.depId)[0];
      this.depForm.controls['depName'].setValue(itemFilter.depName);
      this.depForm.controls['depDescription'].setValue(itemFilter.depDescription);
      this.depForm.controls['depStatus'].setValue(itemFilter.depStatus);
      this.depForm.controls['createdDate'].setValue(itemFilter.createdDate);
      this.depForm.controls['createdBy'].setValue(itemFilter.createdBy);
      this.depForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
      this.depForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
      this.depForm.controls['deleteDate'].setValue(itemFilter.deleteDate);
      this.depForm.controls['deleteBy'].setValue(itemFilter.deleteBy);
    }
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('department.deletemultiple'),
      header: this.translate.instant('department.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('department.confirm'),
      rejectLabel: this.translate.instant('department.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [];
        this.selected.forEach((value: any) => {
          let id = value.depId;
          lstId.push(id);
        });
        let depDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Department/delete', depDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('department.notification'),
                detail: this.translate.instant('department.delsuccess')
              });
              this.getAllDep();
              this.selected = [];
              this.spiner.hide();
            },
            (err: any) => {
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('department.error'),
                detail: err.error.message
              });
              this.spiner.hide();
            }
          );
      }
    });
  }

  delete(dep: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('department.messagedel') + dep.depName + this.translate.instant('department.messageconfirmdel'),
      header: this.translate.instant('department.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('department.confirm'),
      rejectLabel: this.translate.instant('department.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [dep.depId];
        let depDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Department/delete', depDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('department.notification'),
                detail: this.translate.instant('department.delsuccess')
              });
              this.getAllDep();
              this.spiner.hide();
            },
            (err: any) => {
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('department.error'),
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
    this.getAllDep();
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows;
    this.pageSize = event.rows;
    this.getAllDep();
  }

  addData() {
    if (this.depForm.invalid) {
      this.depForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    if (this.action == 'create') {
      this.depForm.controls['createdBy'].setValue(user.id);
      let department = this.depForm.value;
      this.dataService.post('Department/create', department).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('department.notification'),
            detail: this.translate.instant('department.addsuccess')
          });
          this.getAllDep();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('department.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    } else if (this.action == 'edit') {
      this.depForm.controls['updatedBy'].setValue(user.id);
      let department = this.depForm.value;
      this.dataService.put('Department/update', department).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('department.notification'),
            detail: this.translate.instant('department.updatesuccess')
          });
          this.getAllDep();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('department.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
  }

  hideDialog() {
    this.action == '';
    this.depDialog = false;
    this.depForm.reset({
      depId: 0,
      depName: '',
      depDescription: '',
      depStatus: '',
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      deleteBy: '',
      deleteDate: ''
    });
  }
}
