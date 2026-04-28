import { Component } from '@angular/core';

@Component({
    standalone: true,
    selector: 'app-footer',
    template: `<div class="layout-footer">
        Made by
        <a href="https://profile-tuannq.netlify.app/" target="_blank" rel="noopener noreferrer" class="text-primary font-bold hover:underline">Nguyễn Quang Tuấn</a>
    </div>`
})
export class AppFooter {}
