import { Component } from '@angular/core';
import { KanbanComponent } from './components/kanban/kanban.component';
import { RouterOutlet } from '@angular/router';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    KanbanComponent,
    DragDropModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'TaskManagement-UI';
}
