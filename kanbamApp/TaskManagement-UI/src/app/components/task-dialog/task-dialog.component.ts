import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TaskService } from '../../../services/task.service';

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './task-dialog.component.html',
  styleUrls: ['./task-dialog.component.scss']
})
export class TaskDialogComponent implements OnInit {
  taskForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<TaskDialogComponent>,
    private taskService: TaskService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.taskForm = this.fb.group({
      title: ['', [
        Validators.required,
        Validators.maxLength(30),
        Validators.pattern(/.*\S.*/)
      ]],
      description: ['', [
        Validators.maxLength(50)
      ]]
    });
  }

  onSubmit(): void {
    if (this.taskForm.invalid || this.isLoading) {
      return;
    }

    this.isLoading = true;
    this.taskForm.disable();

    const request = this.taskForm.value;

    this.taskService.createTask(request).subscribe({
      next: (createdTask) => {
        this.isLoading = false;
        this.snackBar.open('¡Tarea creada con éxito!', 'Cerrar', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['snackbar-success']
        });
        this.dialogRef.close(createdTask);
      },
      error: (err) => {
        this.isLoading = false;
        this.taskForm.enable(); 
        console.error('Error al crear tarea', err);
        this.snackBar.open('Error al crear la tarea. Por favor, inténtelo de nuevo.', 'Cerrar', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['snackbar-error']
        });
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
