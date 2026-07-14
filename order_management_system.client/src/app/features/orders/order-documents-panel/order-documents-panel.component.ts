import { Component, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { OrderDocument } from '../../../core/models/order-document.model';
import { DocumentsService } from '../../../core/services/documents.service';

@Component({
  selector: 'app-order-documents-panel',
  standalone: false,
  templateUrl: './order-documents-panel.component.html',
  styleUrls: ['./order-documents-panel.component.css']
})
export class OrderDocumentsPanelComponent implements OnChanges {
  @Input() orderId!: number;

  documents: OrderDocument[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(
    private documentsService: DocumentsService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['orderId'] && this.orderId) {
      this.loadDocuments();
    }
  }

  loadDocuments(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.documentsService.getDocumentsForOrder(this.orderId).subscribe({
      next: (documents) => {
        this.documents = documents;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.documents = [];
        this.errorMessage = 'Failed to load documents.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  downloadDocument(document: OrderDocument): void {
    this.documentsService.downloadDocument(document.documentId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = window.document.createElement('a');

        anchor.href = url;
        anchor.download = document.fileName;
        anchor.click();

        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage = 'Failed to download document.';
        this.cdr.markForCheck();
      }
    });
  }

  viewDocument(document: OrderDocument): void {
    this.documentsService.downloadDocument(document.documentId).subscribe({
      next: (blob) => {
        const fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      },
      error: () => {
        this.errorMessage = 'Failed to open document.';
        this.cdr.markForCheck();
      }
    });
  }
}
