import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "../context/AuthContext";
import Dashboard from "../pages/Dashboard";
import { test, expect, vi } from "vitest";

// mock da API
vi.mock("../services/api", () => ({
  default: {
    get: vi.fn(() =>
      Promise.resolve({
        data: [
          {
            id: "1",
            title: "Task 1",
            description: "Desc 1",
            isCompleted: false,
            dueDate: new Date().toISOString(),
          },
        ],
      }),
    ),
  },
}));

function renderDashboard() {
  return render(
    <AuthProvider>
      <BrowserRouter>
        <Dashboard />
      </BrowserRouter>
    </AuthProvider>,
  );
}

test("renders tasks from API", async () => {
  renderDashboard();

  expect(await screen.findByText("Task 1")).toBeInTheDocument();
});
