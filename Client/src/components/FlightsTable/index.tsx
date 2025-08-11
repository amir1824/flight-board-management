import { type FC, useMemo, useState, useEffect } from "react";
import styles from "./index.module.scss";
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  TablePagination,
  LinearProgress,
} from "@mui/material";
import type { FlightModel } from "../../store/dbModels/FlightModel";
import { FlightRow } from "./FlightRow";
import { DeleteDialog } from "../DeleteDialog";
import { FlightsFilterBar } from "./FlightsFilterBar";
import { AnimatePresence } from "framer-motion";

const stickyFirstColSx = {
  position: { xs: "sticky", md: "static" as const },
  left: { xs: 0, md: "auto" as const },
  zIndex: { xs: 2, md: "auto" as const },
  backgroundColor: (t: any) => t.palette.background.paper,
  boxShadow: { xs: "2px 0 0 rgba(0,0,0,0.06)", md: "none" },
};
const stickyHeadColSx = {
  ...stickyFirstColSx,
  top: 0,     // שומר גם על הסטיקיות האנכית של הכותרת
  zIndex: 4,  // גבוה יותר מהגוף כדי שלא יכוסה
};
export interface FlightsTableProps {
  flights: FlightModel[];
  isFetching?: boolean;
  onDelete?: (id: number) => void;
  onAddFlight: () => void;
}

export const FlightsTable: FC<FlightsTableProps> = ({
  flights,
  isFetching = false,
  onAddFlight,
  onDelete,
}) => {
  const [toDelete, setToDelete] = useState<FlightModel | null>(null);
  const [deletingIds, setDeletingIds] = useState<Set<number>>(new Set());

  const askDelete = (id: number) => {
    setToDelete(flights.find((x) => x.id === id) || null);
  };

  const handleConfirmDelete = () => {
    if (toDelete) {
      setDeletingIds((prev) => new Set(prev).add(toDelete.id));
      onDelete?.(toDelete.id);
    }
    setToDelete(null);
  };

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);

  const pagedFlights = useMemo(() => {
    const start = page * rowsPerPage;
    return flights.slice(start, start + rowsPerPage);
  }, [flights, page, rowsPerPage]);

  useEffect(() => {
    const maxPage = Math.max(0, Math.ceil(flights.length / rowsPerPage) - 1);
    if (page > maxPage) {
      setPage(maxPage);
    }
  }, [flights.length, rowsPerPage, page]);

  return (
    <>
      <Paper className={styles.tablePaper} elevation={3}>
        <FlightsFilterBar onAddFlight={onAddFlight} />
        <TableContainer component="div" className={styles.tableContainer} data-cy="flights-table">
          <Table size="medium" stickyHeader>
            <TableHead>
              <TableRow>
                <TableCell sx={stickyHeadColSx} >Flight</TableCell>
                <TableCell>Destination</TableCell>
                <TableCell>Departure</TableCell>
                <TableCell>Gate</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {isFetching && (
                <TableRow>
                  <TableCell colSpan={6} sx={{ p: 0 }}>
                    <LinearProgress />
                  </TableCell>
                </TableRow>
              )}

              <AnimatePresence initial={false}>
                {pagedFlights.length > 0 ? (
                  pagedFlights.map((f) => (
                    <FlightRow
                      key={f.id}
                      flight={f}
                      onDelete={askDelete}
                      firstCellSx={stickyFirstColSx}
                      isDeleting={deletingIds.has(f.id)}
                    />
                  ))
                ) : !isFetching ? (
                  <TableRow>
                    <TableCell colSpan={6}>
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        align="center"
                        sx={{ py: 3 }}
                        data-cy="table-empty"
                      >
                        No flights to display
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : null}
              </AnimatePresence>
            </TableBody>
          </Table>
        </TableContainer>

        <TablePagination
          component="div"
          count={flights.length}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => {
            setRowsPerPage(parseInt(e.target.value, 10));
            setPage(0);
          }}
          rowsPerPageOptions={[10, 25, 50, 100]}
          labelRowsPerPage="Rows:"
          labelDisplayedRows={({ from, to, count }) =>
            `${from}–${to} of ${count}`
          }
          showFirstButton
          showLastButton
          className={styles.pagination}
          data-cy="pagination"
        />
      </Paper>

      <DeleteDialog
        isOpen={!!toDelete}
        title="Delete Flight"
        onClose={() => setToDelete(null)}
        flightNumber={toDelete?.flightNumber ?? ""}
        onDelete={handleConfirmDelete}
      />
    </>
  );
};