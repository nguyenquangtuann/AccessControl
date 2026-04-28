import { SystemConstants } from '@/core/common/system.constants';
import { UserRoles } from '@/core/common/userRole.pipe';
import { DataService } from '@/core/service/data.service';
import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService, ConfirmationService } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { RatingModule } from 'primeng/rating';
import { RippleModule } from 'primeng/ripple';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { RadioButtonModule } from "primeng/radiobutton";
import { CheckboxModule } from 'primeng/checkbox';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
@Component({
  selector: 'app-employee',
  standalone: true,
  imports: [CommonModule, TableModule, FormsModule, ButtonModule, RippleModule, ToastModule,
    ToolbarModule, RatingModule, InputTextModule, InputNumberModule, DialogModule, TagModule,
    InputIconModule, IconFieldModule, ConfirmDialogModule, ReactiveFormsModule, AutoCompleteModule, MessageModule, UserRoles, DatePickerModule, RadioButtonModule, CheckboxModule, TranslateModule],
  templateUrl: './employee.html',
  providers: [MessageService, ConfirmationService, DatePipe]
})
export class Employee implements OnInit {
  keyword: string = '';
  allEmployee: any[] = [];
  page = 0;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  emDialog = false;
  imgDialog = false;
  imgShow: string = '';
  emForm!: FormGroup;
  department: any[] = [];
  filteredDep: any[] = [];
  regency: any[] = [];
  filteredReg: any[] = [];
  action: string = '';
  title: string = '';
  selected: any;
  @ViewChild('dt') dt!: Table;
  loading: boolean = false;

  constructor(
    private dataService: DataService,
    private messageService: MessageService, private datepipe: DatePipe,
    private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService
  ) {
    this.emForm = this.formBuilder.group(
      {
        emId: 0,
        regId: ['', Validators.compose([Validators.required])],
        depId: ['', Validators.compose([Validators.required])],
        emCode: ['', Validators.compose([Validators.required, Validators.maxLength(15)])],
        emName: ['', Validators.compose([Validators.required, Validators.maxLength(100)])],
        emGender: ['M'],
        emBirthdate: '',
        emIdentityNumber: ['', Validators.compose([Validators.pattern('^[0-9]*$'), Validators.minLength(9), Validators.maxLength(12)])],
        emAddress: ['', Validators.compose([Validators.maxLength(400)])],
        emPhone: ['', Validators.compose([Validators.pattern('^(\\+84-?)[0-9]{9}|(086|096|097|098|032|033|034|035|036|037|038|039|088|091|094|083|084|085|081|082|089|090|093|070|079|077|076|078|092|056|058|099|059)[0-9]{7}$'), Validators.minLength(10)])],
        emEmail: ['', Validators.compose([Validators.email, Validators.maxLength(50)])],
        emImage: '',
        emStatus: true,
        editStatus: true,
        devIdSynchronized: '',
        createdDate: '',
        createdBy: '',
        updatedDate: '',
        updatedBy: '',
        deleteBy: '',
        deleteDate: ''
      }
    );
  }

  ngOnInit(): void {
    this.getAllEm();
    this.getAllDep();
    this.getAllReg();
  }

  getAllEm() {
    this.dataService.get('Employee/getListPaging?page=' + this.page + '&pageSize=' + this.pageSize + '&keyword=' + this.keyword).subscribe((data: any) => {
      this.allEmployee = data.items;
      this.totalRecords = data.totalCount;
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employee.error'),
        detail: this.translate.instant('employee.detailget')
      });
    })
  }

  getAllDep() {
    this.dataService.get('Department/getall').subscribe((data: any) => {
      this.department = data;
    }, (err: any) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employee.error'),
        detail: this.translate.instant('employee.detailgetdep')
      });
    });
  }

  filterDep(event: any) {
    const query = event.query.toLowerCase();
    this.filteredDep = this.department.filter(dep =>
      dep.depName.toLowerCase().includes(query)
    );
  }

  getAllReg() {
    this.dataService.get('Regency/getall').subscribe((data: any) => {
      this.regency = data;
    }, (err: any) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employee.error'),
        detail: this.translate.instant('employee.detailgetreg')
      });
    });
  }

  filterReg(event: any) {
    const query = event.query.toLowerCase();
    this.filteredReg = this.regency.filter(reg =>
      reg.regName.toLowerCase().includes(query)
    );
  }

  openDialog(action: string, item?: any) {
    this.emDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('employee.addtitle');
    }
    else {
      this.title = this.translate.instant('employee.edittitle');
      this.emForm.controls['emId'].setValue(item.emId);
      let itemFilter = this.allEmployee.filter((x: any) => x.emId == item.emId)[0];
      this.emForm.controls['emCode'].setValue(itemFilter.emCode);
      this.emForm.controls['emName'].setValue(itemFilter.emName);
      this.emForm.controls['emGender'].setValue(itemFilter.emGender);
      let birthDate = this.convertDateTime(itemFilter.emBirthdate);
      this.emForm.controls['emBirthdate'].setValue(birthDate);
      this.emForm.controls['emIdentityNumber'].setValue(itemFilter.emIdentityNumber);
      this.emForm.controls['emAddress'].setValue(itemFilter.emAddress);
      this.emForm.controls['emPhone'].setValue(itemFilter.emPhone);
      this.emForm.controls['emEmail'].setValue(itemFilter.emEmail);
      this.emForm.controls['emImage'].setValue(itemFilter.emImage);
      this.emForm.controls['devIdSynchronized'].setValue(itemFilter.devIdSynchronized);
      this.emForm.controls['editStatus'].setValue(itemFilter.editStatus);
      this.emForm.controls['emStatus'].setValue(itemFilter.emStatus);
      this.emForm.controls['createdDate'].setValue(itemFilter.createdDate);
      this.emForm.controls['createdBy'].setValue(itemFilter.createdBy);
      this.emForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
      this.emForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
      this.emForm.controls['deleteDate'].setValue(itemFilter.deleteDate);
      this.emForm.controls['deleteBy'].setValue(itemFilter.deleteBy);
      this.emForm.controls['depId'].setValue(
        this.department.filter((dep: any) => dep.depId == itemFilter.depId)[0]
      );

      this.emForm.controls['regId'].setValue(
        this.regency.filter((reg: any) => reg.regId == itemFilter.regId)[0]
      );
    }
  }

  convertDateTime(str: string | null | undefined): Date | null {
    if (!str) {
      return null;
    }
    const [datePart, timePart] = str.split('T');
    const [year, month, day] = datePart.split('-').map(Number);
    const [hours, minutes, seconds] = timePart.split(':').map(Number);
    return new Date(year, month - 1, day, hours, minutes, seconds);
  }

  hideDialog() {
    this.action == '';
    this.emDialog = false;
    this.emForm.reset({
      emId: 0,
      regId: '',
      depId: '',
      emCode: '',
      emName: '',
      emGender: 'M',
      emBirthdate: '',
      emIdentityNumber: '',
      emAddress: '',
      emPhone: '',
      emEmail: '',
      emImage: '',
      emStatus: true,
      editStatus: true,
      devIdSynchronized: '',
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      deleteBy: '',
      deleteDate: ''
    });
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('employee.deletemultiple'),
      header: this.translate.instant('employee.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('employee.confirm'),
      rejectLabel: this.translate.instant('employee.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [];
        this.selected.forEach((value: any) => {
          let id = value.emId;
          lstId.push(id);
        });
        let emDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Employee/delete', emDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('employee.notification'),
                detail: this.translate.instant('employee.delsuccess')
              });
              this.getAllEm();
              this.selected = [];
              this.spiner.hide();
            },
            (err: any) => {
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('employee.error'),
                detail: err.error.message
              });
              this.spiner.hide();
            }
          );
      }
    });
  }

  delete(em: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('employee.messagedel') + em.emName + this.translate.instant('employee.messageconfirmdel'),
      header: this.translate.instant('employee.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('employee.confirm'),
      rejectLabel: this.translate.instant('employee.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [em.emId];
        let emDelete = {
          lstId: JSON.stringify(lstId),
          userId: user.id
        }
        this.dataService
          .deleteData('Employee/delete', emDelete).subscribe(
            (response) => {
              this.messageService.add({
                severity: 'success',
                summary: this.translate.instant('employee.notification'),
                detail: this.translate.instant('employee.delsuccess')
              });
              this.getAllEm();
              this.spiner.hide();
            },
            (err: any) => {
              this.spiner.hide();
              this.messageService.add({
                severity: 'error',
                summary: this.translate.instant('employee.error'),
                detail: err.error.message
              });
            }
          );
      }
    });
  }

  applySearch(event: Event) {
    this.keyword = (event.target as HTMLInputElement).value;
    this.page = 0;
    this.getAllEm();
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows;
    this.pageSize = event.rows;
    this.getAllEm();
  }

  addData() {
    if (this.emForm.invalid) {
      this.emForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    if (this.action == 'create') {
      const formValue = this.emForm.value;
      let employee = {
        regId: this.emForm.controls['regId'].value.regId,
        depId: this.emForm.controls['depId'].value.depId,
        emCode: this.emForm.controls['emCode'].value,
        emName: this.emForm.controls['emName'].value,
        emGender: this.emForm.controls['emGender'].value,
        emBirthdate: this.datepipe.transform(formValue.emBirthdate, 'yyyy-MM-dd'),
        emIdentityNumber: this.emForm.controls['emIdentityNumber'].value,
        emAddress: this.emForm.controls['emAddress'].value,
        emPhone: this.emForm.controls['emPhone'].value,
        emEmail: this.emForm.controls['emEmail'].value,
        emImage: this.emForm.controls['emImage'].value,
        emStatus: this.emForm.controls['emStatus'].value,
        editStatus: this.emForm.controls['editStatus'].value,
        devIdSynchronized: this.emForm.controls['devIdSynchronized'].value,
        createdDate: this.emForm.controls['createdDate'].value,
        createdBy: user.id,
        updatedDate: this.emForm.controls['updatedDate'].value,
        updatedBy: this.emForm.controls['updatedBy'].value,
        deleteBy: this.emForm.controls['deleteBy'].value,
        deleteDate: this.emForm.controls['deleteDate'].value,
      }
      this.dataService.post('Employee/create', employee).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('employee.notification'),
            detail: this.translate.instant('employee.addsuccess')
          });
          this.getAllEm();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('employee.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    } else if (this.action == 'edit') {
      const formValue = this.emForm.value;
      let employee = {
        emId: this.emForm.controls['emId'].value,
        regId: this.emForm.controls['regId'].value.regId,
        depId: this.emForm.controls['depId'].value.depId,
        emCode: this.emForm.controls['emCode'].value,
        emName: this.emForm.controls['emName'].value,
        emGender: this.emForm.controls['emGender'].value,
        emBirthdate: this.datepipe.transform(formValue.emBirthdate, 'yyyy-MM-dd'),
        emIdentityNumber: this.emForm.controls['emIdentityNumber'].value,
        emAddress: this.emForm.controls['emAddress'].value,
        emPhone: this.emForm.controls['emPhone'].value,
        emEmail: this.emForm.controls['emEmail'].value,
        emImage: this.emForm.controls['emImage'].value,
        emStatus: this.emForm.controls['emStatus'].value,
        editStatus: this.emForm.controls['editStatus'].value,
        devIdSynchronized: this.emForm.controls['devIdSynchronized'].value,
        createdDate: this.emForm.controls['createdDate'].value,
        createdBy: this.emForm.controls['createdBy'].value,
        updatedDate: this.emForm.controls['updatedDate'].value,
        updatedBy: user.id,
        deleteBy: this.emForm.controls['deleteBy'].value,
        deleteDate: this.emForm.controls['deleteDate'].value,
      }
      this.dataService.put('Employee/update', employee).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('employee.notification'),
            detail: this.translate.instant('employee.updatesuccess')
          });
          this.getAllEm();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('employee.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
  }

  // Preview image before upload
  imagePreview(e: any) {
    const file = (e.target as HTMLInputElement).files![0];
    const reader = new FileReader();
    reader.onload = () => {
      let base64String = reader.result as string;
      let img = base64String.split('base64,')[1];
      this.emForm.controls['emImage'].setValue(img);
    };
    reader.readAsDataURL(file);
  }

  clearImage() {
    this.emForm.controls['emImage'].setValue('');
  }

  imgShowDialog(item: string) {
    this.imgDialog = true;
    this.imgShow = item;
  }

  hideDialogImg() {
    this.imgDialog = false;
    this.imgShow = '';
  }
}
