import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ActivateCustomerService } from '../../services/activate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';
import { ExportCustomerService } from '../../services/export-customer.service';
import { GetCustomerService } from '../../services/get-customer.service';
import { CustomerListComponent } from './customer-list.component';

describe('CustomerListComponent', () => {
  let component: CustomerListComponent;
  let loadCustomers: ReturnType<typeof vi.fn>;
  let exportCustomers: ReturnType<typeof vi.fn>;
  let totalItems: ReturnType<typeof signal<number>>;

  beforeEach(() => {
    loadCustomers = vi.fn();
    exportCustomers = vi.fn();
    totalItems = signal(0);

    const getCustomerServiceStub = {
      customersSignal: signal([]),
      loadingSignal: signal(false),
      errorSignal: signal<string | null>(null),
      totalItemsSignal: totalItems,
      pageNumberSignal: signal(1),
      pageSizeSignal: signal(10),
      loadCustomers,
    };

    TestBed.configureTestingModule({
      providers: [
        {
          provide: GetCustomerService,
          useValue: getCustomerServiceStub,
        },
        {
          provide: ActivateCustomerService,
          useValue: {
            loadingSignal: signal(false),
            errorSignal: signal<string | null>(null),
          },
        },
        {
          provide: DeleteCustomerService,
          useValue: { deleteCustomer: vi.fn() },
        },
        {
          provide: ExportCustomerService,
          useValue: {
            loadingSignal: signal(false),
            errorSignal: signal<string | null>(null),
            exportCustomers,
          },
        },
        { provide: Router, useValue: { navigate: vi.fn() } },
      ],
    });

    component = TestBed.runInInjectionContext(() => new CustomerListComponent());
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  describe('setSort', () => {
    it('toggles direction when clicking the already-active column, and resets to page 1', () => {
      totalItems.set(25);
      component.currentPage.set(3);
      loadCustomers.mockClear();

      component.setSort('name'); // 'name' is already the default sort column

      expect(component.sortColumn()).toBe('name');
      expect(component.sortDirection()).toBe('desc');
      expect(component.currentPage()).toBe(1);
      expect(loadCustomers).toHaveBeenCalledTimes(1);
      expect(loadCustomers).toHaveBeenCalledWith({
        pageNumber: 1,
        pageSize: 20,
        searchTerm: undefined,
        sortColumn: 'name',
        sortDirection: 'desc',
      });
    });

    it('switches column and resets direction to asc when clicking a different column', () => {
      component.setSort('email');

      expect(component.sortColumn()).toBe('email');
      expect(component.sortDirection()).toBe('asc');
      expect(loadCustomers).toHaveBeenLastCalledWith({
        pageNumber: 1,
        pageSize: 20,
        searchTerm: undefined,
        sortColumn: 'email',
        sortDirection: 'asc',
      });
    });
  });

  describe('goToPage', () => {
    it('clamps above the last page down to totalPages', () => {
      totalItems.set(45); // 45 items / 20 per page = 3 pages
      loadCustomers.mockClear();

      component.goToPage(10);

      expect(component.currentPage()).toBe(3);
      expect(loadCustomers).toHaveBeenLastCalledWith(
        expect.objectContaining({ pageNumber: 3 }),
      );
    });

    it('clamps below page 1 up to 1', () => {
      totalItems.set(25);
      component.currentPage.set(3);
      loadCustomers.mockClear();

      component.goToPage(0);

      expect(component.currentPage()).toBe(1);
      expect(loadCustomers).toHaveBeenLastCalledWith(
        expect.objectContaining({ pageNumber: 1 }),
      );
    });

    it('does nothing when the target page equals the current page', () => {
      loadCustomers.mockClear();

      component.goToPage(1); // already on page 1, totalPages() is 1 with 0 items

      expect(loadCustomers).not.toHaveBeenCalled();
    });
  });

  describe('onSearchInput', () => {
    it('debounces so only the last call within the window triggers a fetch', () => {
      vi.useFakeTimers();

      component.onSearchInput('d');
      vi.advanceTimersByTime(100);
      component.onSearchInput('da');
      vi.advanceTimersByTime(100);
      component.onSearchInput('dan');

      expect(loadCustomers).not.toHaveBeenCalled();

      vi.advanceTimersByTime(299);
      expect(loadCustomers).not.toHaveBeenCalled();

      vi.advanceTimersByTime(1);
      expect(loadCustomers).toHaveBeenCalledTimes(1);
      expect(loadCustomers).toHaveBeenCalledWith({
        pageNumber: 1,
        pageSize: 20,
        searchTerm: 'dan',
        sortColumn: 'name',
        sortDirection: 'asc',
      });
    });

    it('resets to page 1 once the debounced fetch fires', () => {
      vi.useFakeTimers();
      totalItems.set(25);
      component.currentPage.set(2);
      loadCustomers.mockClear();

      component.onSearchInput('dan');
      vi.advanceTimersByTime(300);

      expect(component.currentPage()).toBe(1);
    });
  });

  describe('exportCsv', () => {
    it('exports with the current search term (trimmed) and sort state', () => {
      component.searchTerm.set('  dan  ');
      component.setSort('email');

      component.exportCsv();

      expect(exportCustomers).toHaveBeenCalledWith({
        searchTerm: 'dan',
        sortColumn: 'email',
        sortDirection: 'asc',
      });
    });

    it('omits searchTerm when the search box is blank', () => {
      component.exportCsv();

      expect(exportCustomers).toHaveBeenCalledWith({
        searchTerm: undefined,
        sortColumn: 'name',
        sortDirection: 'asc',
      });
    });
  });
});
