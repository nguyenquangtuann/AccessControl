import { Component } from '@angular/core';
import { AppSpinerService } from '../service/spiner.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-spinner',
    standalone: true,
    imports: [CommonModule, ProgressSpinnerModule],
    template: `<div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.7); display: flex; align-items: center; justify-content: center; z-index: 9999;" *ngIf="spinnerService.loading$ | async">
  <p-progressSpinner></p-progressSpinner>
</div>`
})
export class SpinnerComponent {
    constructor(public spinnerService: AppSpinerService) { }
}