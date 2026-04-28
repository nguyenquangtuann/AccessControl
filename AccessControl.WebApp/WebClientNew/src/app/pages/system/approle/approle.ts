import { SystemConstants } from '@/core/common/system.constants';
import { UserPipe } from '@/core/common/userPipe.pipe';
import { UserRoles } from '@/core/common/userRole.pipe';
import { DataService } from '@/core/service/data.service';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ConfirmationService, MessageService, TreeNode } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TreeModule } from 'primeng/tree';
const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
@Component({
  selector: 'app-approle',
  standalone: true,
  imports: [CommonModule, TableModule, FormsModule, ButtonModule, ToastModule, ToolbarModule, DialogModule, ConfirmDialogModule, ReactiveFormsModule, AutoCompleteModule, MessageModule, InputIconModule, IconFieldModule, InputTextModule, UserRoles, TreeModule, TranslateModule],
  templateUrl: './approle.html',
  providers: [MessageService, ConfirmationService]
})
export class Approle implements OnInit {
  roleDialog = false;
  roleForm!: FormGroup;
  action: string = '';
  title: string = '';
  treeValue: TreeNode[] = [];
  selectedTreeValue: TreeNode[] = [];
  selected: any;
  roleParents: any[] = [];
  filteredRoleParents: any[] = [];

  constructor(private dataService: DataService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService) {
    this.roleForm = this.formBuilder.group({
      id: '',
      name: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
      description: ['', Validators.compose([Validators.required, Validators.maxLength(100)])],
      parentId: '',
      createdBy: '',
      updatedBy: '',
      deletedBy: '',
      createdDate: '',
      updatedDate: '',
      isDeleted: '',
      icon: ['', Validators.compose([Validators.maxLength(100)])],
      link: ['', Validators.compose([Validators.maxLength(100)])],
      activeLink: ['', Validators.compose([Validators.maxLength(100)])],
      orderBy: [''],
    });
  }

  ngOnInit(): void {
    this.getAllRoles();
    this.getRoles();

    this.translate.onLangChange.subscribe(() => {
    this.getRoles(); // reload lại
  });
  }

  getRoles() {
    this.dataService.get('AppRole/getall').subscribe((data: any) => {
      this.roleParents = data.map((item: any) => ({
        ...item, // giữ nguyên các trường khác
        description: this.translate.instant(item.description) // chỉ thay đổi trường description thành bản dịch
      }));
    }, err => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('role.error'),
        detail: this.translate.instant('role.detailgetgroup')
      });
    });
  }

  filterRoleParent(event: any) {
    const query = event.query.toLowerCase();
    this.filteredRoleParents = this.roleParents.filter(r =>
      r.description.toLowerCase().includes(query)
    );
  }

  getAllRoles() {
    this.dataService.get('AppRole/gettreeroles').subscribe((data: any) => {
      this.treeValue = this.mapRolesToTreeNode(data);
    }, err => {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('role.error'),
        detail: this.translate.instant('role.detailgetrole')
      });
    });
  }

  mapRolesToTreeNode(data: any[]): TreeNode[] {
    return data.map(role => ({
      label: role.description,
      key: role.id,
      data: role,
      children: role.childrens ? this.mapRolesToTreeNode(role.childrens) : []
    }));
  }

  openDialog(action: string, item?: any) {
    this.roleDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('role.addtitle');
    }
    else {
      this.title = this.translate.instant('role.edittitle');
      let id: string = item.id;
      this.dataService.get('AppRole/getbyid?id=' + id).subscribe((itemFilter: any) => {
        this.roleForm.controls['id'].setValue(id);
        this.roleForm.controls['name'].setValue(itemFilter.name);
        this.roleForm.controls['description'].setValue(itemFilter.description);
        this.roleForm.controls['createdDate'].setValue(itemFilter.createdDate);
        this.roleForm.controls['createdBy'].setValue(itemFilter.createdBy);
        this.roleForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
        this.roleForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
        this.roleForm.controls['icon'].setValue(itemFilter.icon);
        this.roleForm.controls['link'].setValue(itemFilter.link);
        this.roleForm.controls['activeLink'].setValue(itemFilter.activeLink);
        this.roleForm.controls['orderBy'].setValue(itemFilter.orderBy);
        this.roleForm.controls['parentId'].setValue(this.roleParents.filter((g: any) => g.id == itemFilter.parentId)[0]
        );
      }, err => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('role.error'),
          detail: this.translate.instant('role.detailget')
        });
      });
    }
  }

  hideDialog() {
    this.action == '';
    this.roleDialog = false;
    this.roleForm.reset();
    this.selectedTreeValue = [];
  }

  addData() {
    if (this.roleForm.invalid) {
      this.roleForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    if (this.action == 'create') {
      let role = {
        Name: this.roleForm.controls['name'].value,
        Description: this.roleForm.controls['description'].value,
        ParentId: this.roleForm.controls['parentId'].value != '' ? this.roleForm.controls['parentId'].value.id : null,
        Icon: this.roleForm.controls['icon'].value,
        Link: this.roleForm.controls['link'].value,
        ActiveLink: this.roleForm.controls['activeLink'].value,
        OrderBy: this.roleForm.controls['orderBy'].value,
        CreatedBy: user.id
      }
      this.dataService.post('AppRole/create', role).subscribe(data => {
        this.messageService.add({
          severity: 'success',
          summary: this.translate.instant('role.notification'),
          detail: this.translate.instant('role.addsuccess')
        });
        this.getAllRoles();
        this.getRoles();
        this.spiner.hide();
      }, (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('role.error'),
          detail: err.error.message
        });
        this.spiner.hide();
      });
    }
    else if (this.action == 'edit') {
      let role = {
        Id: this.roleForm.controls['id'].value,
        Name: this.roleForm.controls['name'].value,
        Description: this.roleForm.controls['description'].value,
        ParentId: this.roleForm.controls['parentId'].value != undefined || '' ? this.roleForm.controls['parentId'].value.id : null,
        Icon: this.roleForm.controls['icon'].value,
        Link: this.roleForm.controls['link'].value,
        ActiveLink: this.roleForm.controls['activeLink'].value,
        OrderBy: this.roleForm.controls['orderBy'].value,
        CreatedBy: this.roleForm.controls['createdBy'].value,
        UpdatedBy: user.id,
      }
      this.dataService.put('AppRole/update', role).subscribe(data => {
        this.messageService.add({
          severity: 'success',
          summary: this.translate.instant('role.notification'),
          detail: this.translate.instant('role.updatesuccess')
        });
        this.getAllRoles();
        this.getRoles();
        this.spiner.hide();
      }, (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('role.error'),
          detail: err.error.message
        });
        this.spiner.hide();
      });
    }
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('role.messagedelmultiple'),
      header: this.translate.instant('role.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('role.confirm'),
      rejectLabel: this.translate.instant('role.reject'),
      accept: () => {
        this.spiner.show();
        let lstId = this.selectedTreeValue.map((node: any) => node.key);
        this.dataService.delete('AppRole/delete', 'id', JSON.stringify(lstId)).subscribe(
          (response) => {
            this.messageService.add({
              severity: 'success',
              summary: this.translate.instant('role.notification'),
              detail: this.translate.instant('role.delsuccess')
            });
            this.getAllRoles();
            this.getRoles();
            this.selectedTreeValue = [];
            this.spiner.hide();
          },
          (err: any) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('role.error'),
              detail: err.error.message
            });
            this.spiner.hide();
          }
        );
      }
    });
  }

  delete(role: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('role.messagedel') + role.description + this.translate.instant('role.messageconfirmdel'),
      header: this.translate.instant('role.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('role.confirm'),
      rejectLabel: this.translate.instant('role.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [role.id];
        this.dataService.delete('AppRole/delete', 'id', JSON.stringify(lstId)).subscribe(
          (response) => {
            this.messageService.add({
              severity: 'success',
              summary: this.translate.instant('role.notification'),
              detail: this.translate.instant('role.delsuccess')
            });
            this.getAllRoles();
            this.getRoles();
            this.spiner.hide();
          },
          (err: any) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('role.error'),
              detail: err.error.message
            });
            this.spiner.hide();
          }
        );
      }
    });
  }
}
