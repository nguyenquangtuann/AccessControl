import { SystemConstants } from '@/core/common/system.constants';
import { UserPipe } from '@/core/common/userPipe.pipe';
import { UserRoles } from '@/core/common/userRole.pipe';
import { DataService } from '@/core/service/data.service';
import { AppSpinerService } from '@/layout/service/spiner.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
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
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TreeModule } from 'primeng/tree';

const user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)!);
@Component({
  selector: 'app-appgroup',
  standalone: true,
  imports: [CommonModule, TableModule, FormsModule, ButtonModule, ToastModule, ToolbarModule, DialogModule, TagModule, ConfirmDialogModule, UserPipe, ReactiveFormsModule, AutoCompleteModule, MessageModule, InputIconModule, IconFieldModule, InputTextModule, UserRoles, TreeModule, TranslateModule],
  templateUrl: './appgroup.html',
  providers: [MessageService, ConfirmationService]
})
export class Appgroup implements OnInit {
  keyword: string = '';
  page = 0;
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageSize = this.pageSizeOptions[0];
  totalRecords: number = 0;
  groupDialog = false;
  groupForm!: FormGroup;
  action: string = '';
  title: string = '';
  selected: any;
  @ViewChild('dt') dt!: Table;
  groupData: any[] = [];
  treeValue: TreeNode[] = [];
  selectedTreeValue: TreeNode[] = [];

  constructor(private dataService: DataService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService, private formBuilder: FormBuilder, private spiner: AppSpinerService, private translate: TranslateService) {
    this.groupForm = this.formBuilder.group({
      id: 0,
      groupCode: ['', Validators.compose([Validators.required])],
      name: ['', Validators.compose([Validators.required])],
      createdDate: '',
      createdBy: '',
      updatedDate: '',
      updatedBy: '',
      isDeleted: '',
      roles: [],
    });
  }

  ngOnInit(): void {
    this.loadAllGroups();
    this.loadAllTreeRoles();
  }

  loadAllGroups() {
    this.dataService
      .get(
        'AppGroup/getpaging?page=' + this.page + '&pageSize=' + this.pageSize + '&keyword=' + this.keyword).subscribe(
          (data: any) => {
            this.groupData = data.items;
            this.totalRecords = data.totalCount;
          },
          (err) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('appgroup.error'),
              detail: this.translate.instant('appgroup.detailgetgroup')
            });
          }
        );
  }

  loadAllTreeRoles() {
    this.dataService.get('AppGroup/gettreeroles').subscribe(
      (data: any) => {
        this.treeValue = this.toTreeNode(data);
      },
      (err) => {
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('appgroup.error'),
          detail: this.translate.instant('appgroup.detailgetrole')
        });
      }
    );
  }

  private toTreeNode(data: any[]): TreeNode[] {
    return data.map(item => ({
      key: item.id,
      //label: item.description || item.name,
      label: item.description,
      data: item,
      children: item.childrens ? this.toTreeNode(item.childrens) : []
    }));
  }

  openDialog(action: string, item?: any) {
    this.groupDialog = true;
    this.action = action
    if (action == 'create') {
      this.title = this.translate.instant('appgroup.addtitle');
    }
    else {
      this.title = this.translate.instant('appgroup.edittitle');
      this.groupForm.controls['id'].setValue(item.id);
      let itemFilter = this.groupData.filter((x: any) => x.id == item.id)[0];
      this.groupForm.controls['name'].setValue(itemFilter.name);
      this.groupForm.controls['groupCode'].setValue(itemFilter.groupCode);
      this.groupForm.controls['createdDate'].setValue(itemFilter.createdDate);
      this.groupForm.controls['createdBy'].setValue(itemFilter.createdBy);
      this.groupForm.controls['updatedDate'].setValue(itemFilter.updatedDate);
      this.groupForm.controls['updatedBy'].setValue(itemFilter.updatedBy);
      this.getRoleByGroup(item.id);
    }
  }

  getRoleByGroup(groupId: any) {
    this.dataService.get('AppRole/getlistbygroupid?grId=' + groupId).subscribe((data: any) => {
      const roleIds = data.map((x: any) => x.id ?? x);
      this.selectedTreeValue = this.findNodesByIds(this.treeValue, roleIds);
      // cập nhật trạng thái cha để hiện gạch ngang
      this.treeValue.forEach(node => this.updateParentSelection(node, roleIds));
    });
  }

  private findNodesByIds(tree: TreeNode[], ids: string[]): TreeNode[] {
    let selected: TreeNode[] = [];
    for (let node of tree) {
      if (ids.includes(node.key!)) {
        selected.push(node);
      }
      if (node.children && node.children.length > 0) {
        selected = selected.concat(this.findNodesByIds(node.children, ids));
      }
    }
    return selected;
  }

  private updateParentSelection(node: TreeNode, roleIds: string[]) {
    if (node.children && node.children.length > 0) {
      let checkedChildren = 0;
      node.children.forEach(child => {
        this.updateParentSelection(child, roleIds);
        if (roleIds.includes(child.key!)) {
          checkedChildren++;
        }
        if (child.partialSelected) {
          node.partialSelected = true;
        }
      });

      if (checkedChildren === node.children.length) {
        node.partialSelected = false;
      } else if (checkedChildren > 0) {
        node.partialSelected = true;
      }
    }
  }

  hideDialog() {
    this.action == '';
    this.groupDialog = false;
    this.groupForm.reset();
    this.selectedTreeValue = [];
    this.collapseAll();
  }

  // thu gọn tree
  collapseAll() {
    this.treeValue.forEach((node) => {
      this.expandRecursive(node, false);
    });
  }

  private expandRecursive(node: TreeNode, isExpand: boolean) {
    node.expanded = isExpand;
    if (node.children) {
      node.children.forEach((childNode) => {
        this.expandRecursive(childNode, isExpand);
      });
    }
  }
  // thu gọn tree

  applySearch(event: Event) {
    this.keyword = (event.target as HTMLInputElement).value;
    this.page = 0;
    this.loadAllGroups();
  }

  onLazyLoad(event: any) {
    this.page = event.first / event.rows;
    this.pageSize = event.rows;
    this.loadAllGroups();
  }

  addData() {
    if (this.groupForm.invalid) {
      this.groupForm.markAllAsTouched();
      return;
    }
    this.spiner.show();
    const roleId = this.getSelectedIds();
    if (this.action == 'create') {
      let group = {
        Name: this.groupForm.controls['name'].value,
        GroupCode: this.groupForm.controls['groupCode'].value,
        CreatedBy: user.id,
        Roles: roleId
      }
      this.dataService.post('AppGroup/create', group).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('appgroup.notification'),
            detail: this.translate.instant('appgroup.addsuccess')
          });
          this.loadAllGroups();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('appgroup.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
    else if (this.action == 'edit') {
      let group = {
        Name: this.groupForm.controls['name'].value,
        GroupCode: this.groupForm.controls['groupCode'].value,
        Id: this.groupForm.controls['id'].value,
        CreatedBy: this.groupForm.controls['createdBy'].value,
        UpdatedBy: user.id,
        Roles: roleId
      };
      this.dataService.put('AppGroup/update', group).subscribe(
        (data) => {
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('appgroup.notification'),
            detail: this.translate.instant('appgroup.updatesuccess')
          });
          this.loadAllGroups();
          this.spiner.hide();
        },
        (err: any) => {
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('appgroup.error'),
            detail: err.error.message
          });
          this.spiner.hide();
        }
      );
    }
  }

  getSelectedIds(): { id: string }[] {
    let strRoles: { id: string }[] = [];
    if (this.selectedTreeValue && this.selectedTreeValue.length > 0) {
      this.selectedTreeValue.forEach((node: any) => {
        strRoles.push({ id: node.key });
      });
    }
    return strRoles;
  }

  deleteMultiple() {
    this.confirmationService.confirm({
      message: this.translate.instant('appgroup.messagedelmultiple'),
      header: this.translate.instant('appgroup.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('appgroup.confirm'),
      rejectLabel: this.translate.instant('appgroup.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [];
        this.selected.forEach((value: any) => {
          let id = value.id;
          lstId.push(id);
        });
        this.dataService.delete('AppGroup/delete', 'lstId', JSON.stringify(lstId)).subscribe(
          (response) => {
            this.messageService.add({
              severity: 'success',
              summary: this.translate.instant('appgroup.notification'),
              detail: this.translate.instant('appgroup.delsuccess')
            });
            this.loadAllGroups();
            this.selected = [];
            this.spiner.hide();
          },
          (err: any) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('appgroup.error'),
              detail: err.error.message
            });
            this.spiner.hide();
          }
        );
      }
    });
  }

  delete(group: any) {
    this.confirmationService.confirm({
      message: this.translate.instant('appgroup.messagedel') + group.name + this.translate.instant('appgroup.messageconfirmdel'),
      header: this.translate.instant('appgroup.notification'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('appgroup.confirm'),
      rejectLabel: this.translate.instant('appgroup.reject'),
      accept: () => {
        this.spiner.show();
        let lstId: any[] = [group.id];
        this.dataService.delete('AppGroup/delete', 'lstId', JSON.stringify(lstId)).subscribe(
          (response) => {
            this.messageService.add({
              severity: 'success',
              summary: this.translate.instant('appgroup.notification'),
              detail: this.translate.instant('appgroup.delsuccess')
            });
            this.loadAllGroups();
            this.spiner.hide();
          },
          (err: any) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('appgroup.error'),
              detail: err.error.message
            });
            this.spiner.hide();
          }
        );
      }
    });
  }
}
