import { type FC } from "react";
import { Card, Typography, Grid2 as Grid, Box } from "@mui/material";
import type { FlightModel } from "../../store/dbModels/FlightModel";
import styles from "./index.module.scss";
import { StatusChip } from "../StatusChip";
import { motion, useReducedMotion } from "framer-motion";

const MotionCard = motion(Card);

export interface RecentFlightsCardsProps {
  flights: FlightModel[];
}

export const RecentFlightsCards: FC<RecentFlightsCardsProps> = ({
  flights,
}) => {
  const reduce = useReducedMotion();


  const itemVariants = {
    hidden: { opacity: 0, y: reduce ? 0 : 10 },
    show: {
      opacity: 1,
      y: 0,
      transition: reduce
        ? { duration: 0.2 }
        : { type: "spring" as const, stiffness: 320, damping: 22 },
    },
  };

  const recentFlights = flights
    .slice()
    .sort((a, b) => b.departureTime.unix() - a.departureTime.unix())
    .slice(0, 4);

  return (
    <motion.div className={styles.container} initial="hidden" animate="show">
      <Grid container spacing={1.5}>
        {recentFlights.map((flight) => (
          <Grid key={flight.id}>
            <MotionCard
              className={styles.card}
              variant="outlined"
              variants={itemVariants}
              layout 
              whileHover={!reduce ? { y: -3, scale: 1.01 } : {}}
              whileTap={!reduce ? { scale: 0.99 } : {}}
              transition={{ duration: 0.15 }}
            >
              <Box className={styles.header}>
                <Typography className={styles.flightNumber}>
                  {flight.flightNumber}
                </Typography>
                <div className={styles.statusWrap}>
                  <StatusChip status={flight.status} />
                </div>
              </Box>

              <Typography
                className={styles.destination}
                title={flight.destination}
              >
                {flight.destination}
              </Typography>

              <Typography
                className={styles.gate}
              >{`Gate ${flight.gate}`}</Typography>

              <Typography className={styles.departure}>
                {flight.departureTime.format("YYYY-MM-DD HH:mm")}
              </Typography>
            </MotionCard>
          </Grid>
        ))}
      </Grid>
    </motion.div>
  );
};