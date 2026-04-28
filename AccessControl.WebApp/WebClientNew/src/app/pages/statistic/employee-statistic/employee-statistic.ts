import { DataService } from '@/core/service/data.service';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { AutoCompleteCompleteEvent } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageModule } from 'primeng/message';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';
import { SystemConstants } from '@/core/common/system.constants';
import { FormsModule } from '@angular/forms';
import { UserRoles } from '@/core/common/userRole.pipe';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-employee-statistic',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, ToastModule, ToolbarModule, ConfirmDialogModule, MessageModule, CheckboxModule, TagModule, MultiSelectModule, SelectModule, FormsModule, UserRoles, TranslateModule],
  templateUrl: './employee-statistic.html',
  providers: [MessageService, ConfirmationService, DatePipe]
})
export class EmployeeStatistic implements OnInit {
  allData: any[] = [];
  page = 1;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  disableButton: boolean = true;
  @ViewChild('dt') dt!: Table;
  title: string = '';
  department: any;
  regency: any;
  multiselectSelectedDep: any;
  multiselectSelectedReg: any;
  status: any[] = [];
  statusValue: any;

  constructor(private dataService: DataService, private messageService: MessageService, private spiner: AppSpinerService, private translate: TranslateService) { }

  ngOnInit(): void {
    this.getAllDep();
    this.getAllReg();
    this.loadStatus();
    this.translate.onLangChange.subscribe(() => {
      this.loadStatus();
    });
  }

  loadStatus() {
    this.translate.get(['employeestatistic.deleted', 'employeestatistic.work', 'employeestatistic.all']).subscribe(res => {
      this.status = [
        { id: 0, name: res['employeestatistic.deleted'] },
        { id: 1, name: res['employeestatistic.work'] },
        { id: 2, name: res['employeestatistic.all'] }
      ];
      this.statusValue = this.status.find(x => x.id === 1);
    });
  }

  // Lấy danh sách phòng ban
  getAllDep() {
    this.dataService.get('Department/getall').subscribe((data: any) => {
      this.department = data;
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employeestatistic.error'),
        detail: this.translate.instant('employeestatistic.detailgetdep')
      })
    })
  }

  // Lấy danh sách chức vụ
  getAllReg() {
    this.dataService.get('Regency/getall').subscribe((data: any) => {
      this.regency = data;
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employeestatistic.error'),
        detail: this.translate.instant('employeestatistic.detailgetreg')
      })
    })
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows + 1;
    this.pageSize = event.rows;
    this.loadData();
  }

  loadData() {
    this.spiner.show();

    let lstDepId: string | null = null;
    if (this.multiselectSelectedDep && this.multiselectSelectedDep.length > 0) {
      const ids = this.multiselectSelectedDep.map((x: any) => x.depId);
      lstDepId = JSON.stringify(ids);
    }

    let lstRegId: string | null = null;
    if (this.multiselectSelectedReg && this.multiselectSelectedReg.length > 0) {
      const ids = this.multiselectSelectedReg.map((x: any) => x.regId);
      lstRegId = JSON.stringify(ids);
    }
    let status = this.statusValue ? this.statusValue.id : null;
    this.dataService.get('EmployeeStatistic/getall?depId=' + lstDepId + '&regId=' + lstRegId + '&status=' + status + '&page=' + this.page + '&pageSize=' + this.pageSize).subscribe((data: any) => {
      if (data.data.length == 0) {
        this.messageService.add({
          severity: 'warn',
          summary: this.translate.instant('employeestatistic.notification'),
          detail: this.translate.instant('employeestatistic.nodata')
        })
        this.allData = [];
        this.totalRecords = 0;
        this.spiner.hide();
        this.disableButton = true;
        return;
      }
      this.allData = data.data;
      this.totalRecords = data.total;
      this.disableButton = false;
      this.spiner.hide();
    }, (err) => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employeestatistic.error'),
        detail: this.translate.instant('employeestatistic.detailget')
      })
      this.spiner.hide();
    })
  }

  refresh() {
    this.statusValue = this.status.find(x => x.id === 1);
    this.multiselectSelectedDep = null;
    this.multiselectSelectedReg = null;
    this.disableButton = true;
  }

  exportToExcel() {
    this.spiner.show();
    let lstDepId: string | null = null;
    if (this.multiselectSelectedDep && this.multiselectSelectedDep.length > 0) {
      const ids = this.multiselectSelectedDep.map((x: any) => x.depId);
      lstDepId = JSON.stringify(ids);
    }

    let lstRegId: string | null = null;
    if (this.multiselectSelectedReg && this.multiselectSelectedReg.length > 0) {
      const ids = this.multiselectSelectedReg.map((x: any) => x.regId);
      lstRegId = JSON.stringify(ids);
    }
    let status = this.statusValue ? this.statusValue.id : null;
    this.dataService.post('EmployeeStatistic/exportex?depId=' + lstDepId + '&regId=' + lstRegId + '&status=' + status).subscribe((data: any) => {
      window.location.href = data.result;
      this.spiner.hide();
    }, (err) => {
      this.spiner.hide();
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('employeestatistic.error'),
        detail: this.translate.instant('employeestatistic.errorexport')
      })
    })
  }
}
