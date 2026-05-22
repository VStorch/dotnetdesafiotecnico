import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import TaskCard from "../components/TaskCard";
import { expect, test, vi } from "vitest";

const task = {
  id: "1",
  title: "Test Task",
  description: "Test Description",
  isCompleted: false,
  dueDate: new Date().toISOString(),
};

test("renders task", () => {
  render(
    <TaskCard
      task={task}
      onDelete={() => {}}
      onToggle={() => {}}
      onEdit={() => {}}
    />,
  );

  expect(screen.getByText("Test Task")).toBeInTheDocument();
});

test("calls delete button", async () => {
  const onDelete = vi.fn();

  render(
    <TaskCard
      task={task}
      onDelete={onDelete}
      onToggle={() => {}}
      onEdit={() => {}}
    />,
  );

  await userEvent.click(screen.getByText("Delete"));

  expect(onDelete).toHaveBeenCalledWith("1");
});
