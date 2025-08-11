import {
  TableCell,
  TableRow,
  Typography,
  Tooltip,
  IconButton,
  CircularProgress,
} from "@mui/material";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import { motion } from "framer-motion";
import { keyframes } from "@mui/system";
import { useEffect, useRef, useState, memo } from "react";
import type { FlightModel } from "../../../store/dbModels/FlightModel";
import { StatusChip } from "../../StatusChip";

const MotionTableRow = motion(TableRow);

const statusFlash = keyframes`
  0%   { background-color: rgba(34,197,94,0.22); }
  100% { background-color: transparent; }
`;

type Props = {
  flight: FlightModel;
  onDelete?: (id: number) => void;
  firstCellSx?: any;
  isDeleting?: boolean;
};

export const FlightRow = memo(function FlightRow({
  flight,
  onDelete,
  firstCellSx,
  isDeleting = false,
}: Props) {

  const prevStatusRef = useRef(flight.status);
  const [flashOn, setFlashOn] = useState(false);

  useEffect(() => {
    if (prevStatusRef.current !== flight.status) {
      setFlashOn(true);
      const t = setTimeout(() => setFlashOn(false), 600);
      prevStatusRef.current = flight.status;
      return () => clearTimeout(t);
    }
  }, [flight.status]);


  return (
    <MotionTableRow
      layout 
      initial={{ opacity: 0, y: -8 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: 8 }}
      transition={{ duration: 0.25 }}
      hover
    >
      <TableCell sx={firstCellSx}>
        <Typography variant="body2">{flight.flightNumber}</Typography>
      </TableCell>

      <TableCell>{flight.destination}</TableCell>

      <TableCell>{flight.departureTime.format("YYYY-MM-DD HH:mm")}</TableCell>

      <TableCell>
        <Typography>{flight.gate}</Typography>
      </TableCell>

      <TableCell
        sx={{
          borderRadius: 1,
          animation: flashOn ? `${statusFlash} 600ms ease-out` : "none",
          transition: "background-color 600ms ease, color 600ms ease",
        }}
      >
        <motion.div
          key={flight.status}
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.2 }}
          style={{ display: "inline-block" }}
        >
          <StatusChip status={flight.status} />
        </motion.div>
      </TableCell>

      <TableCell align="right">
        <Tooltip title={onDelete ? "Delete flight" : ""}>
          <span>
            <IconButton
              size="small"
              color="error"
              onClick={() => onDelete?.(flight.id)}
              disabled={!onDelete || isDeleting}
              data-cy={`delete-flight-${flight.id}`}
            >
              {isDeleting ? (
                <CircularProgress size={18} />
              ) : (
                <DeleteOutlineIcon fontSize="small" />
              )}
            </IconButton>
          </span>
        </Tooltip>
      </TableCell>
    </MotionTableRow>
  );
});
