import { useEffect, useMemo, useState } from 'react';
import type { FormEvent } from 'react';

type Employee = {
  id: number;
  name: string;
  dateOfBirth: string;
  phoneNumber: string;
  createdAtUtc: string;
};

type EmployeeForm = {
  name: string;
  dateOfBirth: string;
  phoneNumber: string;
};

const initialForm: EmployeeForm = {
  name: '',
  dateOfBirth: '',
  phoneNumber: ''
};

const pageSize = 10;

export default function App() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [form, setForm] = useState<EmployeeForm>(initialForm);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const filteredEmployees = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLocaleLowerCase();
    if (!normalizedSearch) {
      return employees;
    }

    return employees.filter((employee) =>
      [employee.name, employee.dateOfBirth, employee.phoneNumber]
        .some((value) => value.toLocaleLowerCase().includes(normalizedSearch))
    );
  }, [employees, searchTerm]);
  const totalPages = Math.max(1, Math.ceil(filteredEmployees.length / pageSize));
  const page = Math.min(currentPage, totalPages);
  const pageEmployees = filteredEmployees.slice((page - 1) * pageSize, page * pageSize);
  const firstVisibleEmployee = filteredEmployees.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const lastVisibleEmployee = Math.min(page * pageSize, filteredEmployees.length);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/employees');

      if (!response.ok) {
        throw new Error('Unable to load employees.');
      }

      const data = (await response.json()) as Employee[];
      setEmployees(data);
      setError('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadEmployees();
  }, []);

  const handleChange = (field: keyof EmployeeForm, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();

    if (!form.name || !form.dateOfBirth || !form.phoneNumber) {
      setError('Please complete all fields before submitting.');
      return;
    }

    try {
      setSubmitting(true);
      setError('');

      const response = await fetch('/api/employees', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name: form.name,
          dateOfBirth: form.dateOfBirth,
          phoneNumber: form.phoneNumber
        })
      });

      if (!response.ok) {
        const payload = await response.json().catch(() => null);
        throw new Error(
          payload?.errors?.[0] ?? 'The employee could not be created.'
        );
      }

      setForm(initialForm);
      await loadEmployees();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (employee: Employee) => {
    const confirmed = window.confirm(`Are you sure you want to delete ${employee.name}?`);

    if (!confirmed) {
      return;
    }

    try {
      const response = await fetch(`/api/employees/${employee.id}`, {
        method: 'DELETE'
      });

      if (!response.ok) {
        throw new Error('Employee could not be removed.');
      }

      setEmployees((current) => current.filter((item) => item.id !== employee.id));
      setError('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.');
    }
  };

  return (
    <div className="page-shell">
      <header className="topbar">
        <div className="brand">
          <span className="brand-mark">E</span>
          <div>
            <div className="brand-name">EmployeeHub</div>
            <small>Operations dashboard</small>
          </div>
        </div>
      </header>

      <main className="layout">
        <section className="panel panel--wide">
          <div className="panel-header">
            <div>
              <p className="eyebrow">Overview</p>
              <h1>Employees</h1>
            </div>
            <span className="chip">{employees.length} total</span>
          </div>

          {error ? <div className="alert">{error}</div> : null}

          <div className="list-toolbar">
            <label className="search-field">
              <span>Search employees</span>
              <input
                type="search"
                value={searchTerm}
                onChange={(event) => {
                  setSearchTerm(event.target.value);
                  setCurrentPage(1);
                }}
                placeholder="Search by name, date, or phone"
              />
            </label>
            <span className="results-count" aria-live="polite">
              {filteredEmployees.length} {filteredEmployees.length === 1 ? 'result' : 'results'}
            </span>
          </div>

          {loading ? (
            <div className="table-skeleton" aria-live="polite">
              <div />
              <div />
              <div />
            </div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Date of birth</th>
                    <th>Phone</th>
                    <th>Created</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredEmployees.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="empty-state">
                        {employees.length === 0
                          ? 'No employees have been added yet.'
                          : 'No employees match your search.'}
                      </td>
                    </tr>
                  ) : (
                    pageEmployees.map((employee) => (
                      <tr key={employee.id}>
                        <td>{employee.name}</td>
                        <td>{new Date(`${employee.dateOfBirth}T00:00:00`).toLocaleDateString()}</td>
                        <td>{employee.phoneNumber}</td>
                        <td>{new Date(employee.createdAtUtc).toLocaleDateString()}</td>
                        <td>
                          <button
                            type="button"
                            className="danger-button"
                            onClick={() => void handleDelete(employee)}
                          >
                            Delete
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}

          {!loading && filteredEmployees.length > 0 ? (
            <div className="pagination">
              <span className="pagination-summary">
                Showing {firstVisibleEmployee}–{lastVisibleEmployee} of {filteredEmployees.length}
              </span>
              <div className="pagination-controls">
                <button
                  type="button"
                  className="pagination-button"
                  onClick={() => setCurrentPage(page - 1)}
                  disabled={page === 1}
                  aria-label="Previous page"
                >
                  Previous
                </button>
                <span className="pagination-page" aria-current="page">
                  Page {page} of {totalPages}
                </span>
                <button
                  type="button"
                  className="pagination-button"
                  onClick={() => setCurrentPage(page + 1)}
                  disabled={page === totalPages}
                  aria-label="Next page"
                >
                  Next
                </button>
              </div>
            </div>
          ) : null}
        </section>

        <aside className="panel">
          <div className="panel-header panel-header--compact">
            <div>
              <p className="eyebrow">Add employee</p>
              <h2>New record</h2>
            </div>
          </div>

          <form className="employee-form" onSubmit={(event) => void handleSubmit(event)}>
            <label>
              Full name
              <input
                type="text"
                value={form.name}
                onChange={(event) => handleChange('name', event.target.value)}
                placeholder="Full name"
              />
            </label>

            <label>
              Date of birth
              <input
                type="date"
                value={form.dateOfBirth}
                onChange={(event) => handleChange('dateOfBirth', event.target.value)}
              />
            </label>

            <label>
              Phone number
              <input
                type="tel"
                value={form.phoneNumber}
                onChange={(event) => handleChange('phoneNumber', event.target.value)}
                placeholder="+1 (555) 123-4567"
              />
            </label>

            <button type="submit" className="primary-button" disabled={submitting}>
              {submitting ? 'Saving…' : 'Save employee'}
            </button>
          </form>
        </aside>
      </main>
    </div>
  );
}
